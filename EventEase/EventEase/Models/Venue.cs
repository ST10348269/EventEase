using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(200, ErrorMessage = "Venue name cannot exceed 200 characters.")]
        public string VenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(300, ErrorMessage = "Location cannot exceed 300 characters.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000.")]
        public int Capacity { get; set; }

        // Stored URL (from Azure Blob or existing URL)
        public string? ImageUrl { get; set; }

        // Not mapped — used only for file upload in forms
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
