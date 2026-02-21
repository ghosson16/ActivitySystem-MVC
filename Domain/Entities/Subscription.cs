using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ActivitySystem.Domain.Entities
{
    public class Subscription
    {
        public int Id { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [Required]
        public DateTime SubscriptionDate { get; set; } = DateTime.Now;

        public bool IsAttended { get; set; } = false;

        [Required]
        public string GuestId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser Guest { get; set; }

        [JsonIgnore]
        public virtual Activity Activity { get; set; }

    }
}
