namespace TravelTracker.Models
{
    public class Trip
    {
        public int TripId { get; set; }
        public string Destination { get; set; } = string.Empty;

        // Foreign Keys
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;

        public int TripTypeId { get; set; }
        public TripType TripType { get; set; } = null!;

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;

        // Trip details
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public int Rating { get; set; } // 1-10
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}