using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        public DateTime BookingDate { get; set; }

        // EVENT FK
        [Required(ErrorMessage = "Please select an event.")]
        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public Event? Event { get; set; }

        // VENUE FK
        [Required(ErrorMessage = "Please select a venue.")]
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public Venue? Venue { get; set; }
    }
}
