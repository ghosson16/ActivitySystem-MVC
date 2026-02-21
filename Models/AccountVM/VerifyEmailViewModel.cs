using System.ComponentModel.DataAnnotations;

namespace ActivitySystem.Models.AccountVM
{
    public class VerifyEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
