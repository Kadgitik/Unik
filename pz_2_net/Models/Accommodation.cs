namespace TravelTracker.Models
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}