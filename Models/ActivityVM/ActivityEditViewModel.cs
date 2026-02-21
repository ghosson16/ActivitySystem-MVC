using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models.ActivityVM
{
    public class ActivityEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 5)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please select an activity type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Date and time are required")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public string? ImageUrl { get; set; }

        public string? OrganizerId { get; set; }

        public IFormFile? FormFile { get; set; }

        public bool DeleteImage { get; set; }
    }
}
