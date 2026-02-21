using ActivitySystem.Domain.Entities;
using ActivitySystem.Infrastructure.Repositories;
using ActivitySystem.Models.SubscriptionVM; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ActivitySystem.Controllers
{
    public class SubscriptionController : Controller
    {
        #region Fields & Constructor
        private readonly ISubscriptionRepository _subRepo;
        private readonly IActivityRepository _activityRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(
            ISubscriptionRepository subRepo,
            IActivityRepository activityRepo,
            UserManager<ApplicationUser> userManager)
        {
            _subRepo = subRepo;
            _activityRepo = activityRepo;
            _userManager = userManager;
        }
        #endregion

        #region Guest Actions (Join & My Subscriptions)
        [HttpPost]
        [Authorize(Roles = "Guest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int activityId)
        {
            var userId = _userManager.GetUserId(User);
            var activity = await _activityRepo.GetByIdAsync(activityId);

            if (activity == null) return NotFound();

            if (await _subRepo.IsSubscribedAsync(userId, activityId))
            {
                TempData["Error"] = "Already joined!";
                return RedirectToAction("Details", "Activity", new { id = activityId });
            }

            var currentSubscribers = await _subRepo.GetSubscriptionsByActivityAsync(activityId);
            if (currentSubscribers.Count() >= activity.Capacity)
            {
                TempData["Error"] = "Activity is full!";
                return RedirectToAction("Details", "Activity", new { id = activityId });
            }

            var subscription = new Subscription { ActivityId = activityId, GuestId = userId };
            await _subRepo.AddAsync(subscription);

            if (await _subRepo.SaveAsync())
                TempData["Success"] = "Joined successfully!";
            else
                TempData["Error"] = "Error occurred.";

            return RedirectToAction("Details", "Activity", new { id = activityId });
        }

        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> MySubscriptions()
        {
            var userId = _userManager.GetUserId(User);
            var subscriptions = await _subRepo.GetSubscriptionsByGuestAsync(userId);

            var model = subscriptions.Select(s => new MySubscriptionsViewModel
            {
                ActivityId = s.ActivityId,
                ActivityTitle = s.Activity?.Title ?? "N/A",
                Location = s.Activity?.Location ?? "N/A",
                ActivityDate = s.Activity?.Date ?? DateTime.Now,
                IsActivityDeleted = s.Activity?.IsDeleted ?? false,
                IsAttended = s.IsAttended
            }).ToList();

            return View(model);
        }
        #endregion

        #region Organizer Actions (Attendance Management)
        [HttpPost]
        [Authorize(Roles = "Guest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unsubscribe(int activityId)
        {
            var userId = _userManager.GetUserId(User);
            if (!await _subRepo.IsSubscribedAsync(userId, activityId))
            {
                TempData["Error"] = "Subscription not found.";
                return RedirectToAction(nameof(MySubscriptions));
            }

            await _subRepo.DeleteAsync(userId, activityId);
            if (await _subRepo.SaveAsync())
                TempData["Success"] = "Unsubscribed successfully.";
            else
                TempData["Error"] = "Error occurred.";

            return RedirectToAction(nameof(MySubscriptions));
        }


        [HttpGet]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> AttendanceReport(int id)
        {
            var activity = await _activityRepo.GetByIdAsync(id);
            if (activity == null) return NotFound();

            var subscriptions = await _subRepo.GetSubscriptionsByActivityAsync(id);

            var model = new AttendanceReportViewModel
            {
                ActivityId = activity.Id,
                ActivityTitle = activity.Title,
                ActivityDate = activity.Date,
                Attendees = subscriptions.Select(s => new AttendeeItemViewModel
                {
                    GuestId = s.GuestId,
                    GuestName = s.Guest?.Name ?? "Unknown",
                    GuestPhoneNumber = s.Guest?.PhoneNumber ?? "No Phone",
                    IsAttended = s.IsAttended
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPresent(int activityId, string guestId)
        {
            await _subRepo.MarkAsAttendedAsync(activityId, guestId);
            if (await _subRepo.SaveAsync())
                TempData["Success"] = "Attendance recorded!";
            else
                TempData["Error"] = "Failed to record attendance.";

            return RedirectToAction(nameof(AttendanceReport), new { id = activityId });
        }
        #endregion
    }
}