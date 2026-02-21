using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models.AccountVM
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Range(12, 90, ErrorMessage = "Age must be between 12 and 90")]
        [Display(Name = "Age")]
        public int? Age { get; set; }

        [Display(Name = "Organization Description")]
        public string? Description { get; set; }
    }
}
