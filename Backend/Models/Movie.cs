using System.ComponentModel.DataAnnotations;

namespace VideoRentalService.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Type { get; set; } // 'DVD' or 'Electronic'

        public DateTime CreationTime { get; set; }
        public DateTime UpdateTime { get; set; }

        // Navigation property
        public ICollection<Rental> Rentals { get; set; } // A movie can appear in multiple rentals
    }
}
