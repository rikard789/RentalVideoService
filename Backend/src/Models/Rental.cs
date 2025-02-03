using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoRentalService.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } // Foreign key for User
        [ForeignKey("Movie")]
        public int MovieId { get; set; } // Foreign key for Movie

        [Required]
        public DateTime RentalDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public bool IsDamaged { get; set; }

        public decimal? Penalty { get; set; } // isDamaged == true -> how big penalty $ amount
        public bool? IsPaid { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime UpdateTime { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Movie Movie { get; set; }
    }
}
