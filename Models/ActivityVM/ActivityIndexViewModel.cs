namespace ActivitySystem.Models.ActivityVM
{
    public class ActivityIndexViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string? ImageUrl { get; set; }
        public int Capacity { get; set; }
    }
}
