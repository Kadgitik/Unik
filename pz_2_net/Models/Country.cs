namespace TravelTracker.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}