namespace ActivitySystem.Models.ActivityVM
{
    public class ActivityDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string? ImageUrl { get; set; }
        public int Capacity { get; set; }

        public string OrganizerId { get; set; }
        public string OrganizerName { get; set; }
        public string? OrganizerPhoneNumber { get; set; }

        public int RegisteredCount { get; set; }
        public int AvailableSpots => Capacity - RegisteredCount;
        public bool IsFull => AvailableSpots <= 0;
        public double FillPercentage => Capacity > 0 ? (double)RegisteredCount / Capacity * 100 : 0;

        public bool IsCurrentUserSubscribed { get; set; }
    }
}

