using ActivitySystem.Domain.Entities;
using ActivitySystem.Infrastructure.Repositories;
using ActivitySystem.Models;
using ActivitySystem.Models.DashboardVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ActivitySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISubscriptionRepository _subRepo;
        private readonly IActivityRepository _activityRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController (
            ISubscriptionRepository subRepo,
            IActivityRepository activityRepo,
            UserManager<ApplicationUser> userManager)
        {
            _subRepo = subRepo;
            _activityRepo = activityRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.IsInRole("Organizer"))
            {
                return RedirectToAction("OrganizerDashboard");
            }
            else if (User.IsInRole("Guest"))
            {
                return RedirectToAction("GuestDashboard");
            }

            return View(); 
        }

        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> OrganizerDashboard()
        {
            var userId = _userManager.GetUserId(User);
            var activities = await _activityRepo.GetActivitiesByOrganizerAsync(userId);
            int totalSubscribers = await _subRepo.GetTotalParticipantsByOrganizerAsync(userId);
            int totalCapacity = activities.Sum(a => a.Capacity);

            double engagement = totalCapacity > 0
                ? (double)totalSubscribers / totalCapacity * 100
                : 0;

            var model = new OrganizerDashboardViewModel
            {
                ActiveEventsCount = activities.Count(a => a.Date >= DateTime.Now && !a.IsDeleted),
                TotalParticipantsCount = totalSubscribers,
                EngagementRate = Math.Round(engagement, 1)
            };

            return View(model);
        }

        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> GuestDashboard()
        {
            var userId = _userManager.GetUserId(User);
            var subscriptions = await _subRepo.GetSubscriptionsByGuestAsync(userId);

            var model = new GuestDashboardViewModel
            {
                UpcomingActivitiesCount = subscriptions.Count(s =>
                            s.Activity != null && !s.Activity.IsDeleted &&
                            s.Activity.Date >= DateTime.Now && !s.IsAttended),

                TotalAttendedCount = subscriptions.Count(s =>
                            s.Activity != null && !s.Activity.IsDeleted && s.IsAttended)
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
