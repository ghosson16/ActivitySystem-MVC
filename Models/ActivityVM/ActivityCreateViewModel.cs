using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models.ActivityVM
{
    public class ActivityCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters")]
        [Display(Name = "Activity Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please select an activity type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000 guests")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Date and time are required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Activity Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Activity Image")]
        public IFormFile? FormFile { get; set; }

        public string? ImageUrl { get; set; }
    }

}

