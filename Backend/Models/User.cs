using System.ComponentModel.DataAnnotations;

namespace VideoRentalService.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // 'User' or 'Employee'

        public DateTime? CreationTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        // Navigation property
        public ICollection<Rental>? Rentals { get; set; } // A user can have many rentals
    }
}
