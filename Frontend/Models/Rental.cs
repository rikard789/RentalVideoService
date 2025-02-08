using System;

namespace Frontend.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsDamaged { get; set; }
        public decimal? Penalty { get; set; }
        public bool? IsPaid { get; set; }
        public Movie Movie { get; set; } = new Movie();

        public string MovieTitle => Movie?.Title ?? "Unknown";
        public string MovieType => Movie?.Type ?? "Unknown";
    }
}
