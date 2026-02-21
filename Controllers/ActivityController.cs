using ActivitySystem.Domain.Entities;
using ActivitySystem.Infrastructure.Repositories;
using ActivitySystem.Models.ActivityVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ActivitySystem.Controllers
{

    public class ActivityController : Controller
    {
        #region Fields & Constructor
        private readonly IActivityRepository _activityRepo;
        private readonly ISubscriptionRepository _subRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ActivityController(IActivityRepository activityRepo,
            ISubscriptionRepository subRepo,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _subRepo = subRepo;
            _activityRepo = activityRepo;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region Details
        public async Task<IActionResult> Index()
        {
            IEnumerable<Activity> activities;

            if (User.Identity.IsAuthenticated && User.IsInRole("Organizer"))
            {
                var userId = _userManager.GetUserId(User);
                activities = await _activityRepo.GetActivitiesByOrganizerAsync(userId);
            }
            else
            {
                var allActivities = await _activityRepo.GetAllActivitiesAsync();
                activities = allActivities.Where(a => a.Date >= DateTime.Now && !a.IsDeleted);
            }

            var model = activities.Select(a => new ActivityIndexViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Location = a.Location,
                Date = a.Date,
                ImageUrl = a.ImageUrl,
                Capacity = a.Capacity
            });

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var activity = await _activityRepo.GetByIdAsync(id);
            if (activity == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            var model = new ActivityDetailsViewModel
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Location = activity.Location,
                Date = activity.Date,
                ImageUrl = activity.ImageUrl,
                Capacity = activity.Capacity,
                OrganizerId = activity.OrganizerId,
                OrganizerName = activity.Organizer?.Name ?? "Administrator",
                OrganizerPhoneNumber = activity.Organizer?.PhoneNumber,
                RegisteredCount = activity.Subscriptions?.Count ?? 0,
                IsCurrentUserSubscribed = userId != null && await _subRepo.IsSubscribedAsync(userId, id)
            };

            return View(model);
        }
        #endregion

        #region Create

        [HttpGet]
        [Authorize(Roles = "Organizer")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityCreateViewModel model)
        {
            if (model.Date <= DateTime.Now) ModelState.AddModelError("Date", "Date must be in the future.");

            if (ModelState.IsValid)
            {
                var activity = new Activity
                {
                    Title = model.Title,
                    Type = model.Type,
                    Description = model.Description,
                    Location = model.Location,
                    Date = model.Date,
                    Capacity = model.Capacity,
                    OrganizerId = _userManager.GetUserId(User)
                };

                if (model.FormFile != null && model.FormFile.Length > 0)
                {
                    string extension = Path.GetExtension(model.FormFile.FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "activities");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.FormFile.CopyToAsync(stream);
                            activity.ImageUrl = "/images/activities/" + uniqueFileName;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImageUrl", "Only .jpg, .jpeg, and .png files are allowed.");
                        return View(activity);
                    }
                }

                await _activityRepo.AddAsync(activity);
                await _activityRepo.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region delete
        [HttpPost]
        [Authorize(Roles = "Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _activityRepo.GetByIdAsync(id);
            if (activity == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);

            if (activity.OrganizerId == currentUserId)
            {
                await _activityRepo.DeleteAsync(id);
                await _activityRepo.SaveAsync();
            }
            else
            {
                return Forbid();
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _activityRepo.GetByIdAsync(id);
            if (activity == null || activity.OrganizerId != _userManager.GetUserId(User)) return Forbid();

            var model = new ActivityEditViewModel
            {
                Id = activity.Id,
                Title = activity.Title,
                Type = activity.Type,
                Description = activity.Description,
                Location = activity.Location,
                Date = activity.Date,
                Capacity = activity.Capacity,
                ImageUrl = activity.ImageUrl
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Organizer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActivityEditViewModel model)
        {
            var activity = await _activityRepo.GetByIdAsync(model.Id);

            if (activity == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (activity.OrganizerId != userId) return Forbid();

            if (model.Date <= DateTime.Now)
            {
                ModelState.AddModelError("Date", "Date must be in the future.");
            }

            if (ModelState.IsValid)
            {
                activity.Title = model.Title;
                activity.Type = model.Type;
                activity.Description = model.Description;
                activity.Location = model.Location;
                activity.Date = model.Date;
                activity.Capacity = model.Capacity;

                if (model.DeleteImage)
                {
                    activity.ImageUrl = null;
                }

                if (model.FormFile != null && model.FormFile.Length > 0)
                {
                    string extension = Path.GetExtension(model.FormFile.FileName).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + extension;
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "activities");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.FormFile.CopyToAsync(stream);
                            activity.ImageUrl = "/images/activities/" + uniqueFileName;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("FormFile", "Only .jpg, .jpeg, and .png files are allowed.");
                        return View(model);
                    }
                }

                await _activityRepo.UpdateAsync(activity);
                await _activityRepo.SaveAsync();
                return RedirectToAction("Details", new { id = activity.Id });
            }

            model.ImageUrl = activity.ImageUrl;
            return View(model);
        }
        #endregion
    }
}