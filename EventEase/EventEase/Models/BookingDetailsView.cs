namespace EventEase.Models
{
    /// <summary>
    /// ViewModel that mirrors the SQL View vw_BookingDetails.
    /// Used for the enhanced bookings display with search.
    /// </summary>
    public class BookingDetailsView
    {
        public int BookingID { get; set; }
        public DateTime BookingDate { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? EventDescription { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public string? VenueLocation { get; set; }
        public int VenueCapacity { get; set; }
        public string? VenueImageUrl { get; set; }
        public string? EventImageUrl { get; set; }
    }
}
