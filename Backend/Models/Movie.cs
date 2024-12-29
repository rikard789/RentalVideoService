using System.ComponentModel.DataAnnotations;

namespace VideoRentalService.Models
{
    public class Movie
    {
        [Key]
        public int FilmId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Type { get; set; } // 'DVD' or 'Electronic'
    }
}
