using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActivitySystem.Domain.Entities
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        public string Type { get; set; } 

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Capacity { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? ImageUrl { get; set; }

        public string OrganizerId { get; set; }

        public virtual ApplicationUser Organizer { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public bool IsDeleted { get; set; } = false;

    }
}
