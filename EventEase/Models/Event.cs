using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        public string EventName { get; set; }

        public DateTime EventDate { get; set; }

        public string Description { get; set; }

        // FOREIGN KEY
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public Venue Venue { get; set; }
    }
}