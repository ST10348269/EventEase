using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        public DateTime BookingDate { get; set; }

        // EVENT FK
        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public Event Event { get; set; }

        // VENUE FK
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public Venue Venue { get; set; }
    }
}