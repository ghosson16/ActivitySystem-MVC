namespace ActivitySystem.Models.SubscriptionVM
{
    public class MySubscriptionsViewModel
    {
        public int ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public string Location { get; set; }
        public DateTime ActivityDate { get; set; }
        public bool IsActivityDeleted { get; set; }
        public bool IsAttended { get; set; }
    }
}
