namespace ActivitySystem.Models.SubscriptionVM
{
    public class AttendanceReportViewModel
    {
        public int ActivityId { get; set; }
        public string ActivityTitle { get; set; }
        public DateTime ActivityDate { get; set; }
        public List<AttendeeItemViewModel> Attendees { get; set; } = new();
    }

    public class AttendeeItemViewModel
    {
        public string GuestId { get; set; }
        public string GuestName { get; set; }
        public string? GuestPhoneNumber { get; set; }
        public bool IsAttended { get; set; }
    }
}
