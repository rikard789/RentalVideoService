using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoRentalService.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Film")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [Required]
        public DateTime RentalDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public bool IsDamaged { get; set; }

        public decimal? Penalty { get; set; } // isDamaged == true -> how big penalty $ amount
    }
}
