using System;

namespace Frontend.Models
{
    public class Rental
    {
        // Identyfikator wypożyczenia
        public int RentalId { get; set; }

        // Identyfikator użytkownika, który wypożyczył film
        public int UserId { get; set; }

        // Identyfikator filmu, który został wypożyczony
        public int MovieId { get; set; }

        // Data wypożyczenia
        public DateTime RentalDate { get; set; }

        // Data zwrotu (może być pusta, jeśli nie zwrócono jeszcze filmu)
        public DateTime? ReturnDate { get; set; }

        // Flaga informująca, czy film jest uszkodzony
        public bool IsDamaged { get; set; }

        // Kara za uszkodzenie, jeśli film jest uszkodzony
        public decimal? Penalty { get; set; }

        // Flaga informująca, czy kara została opłacona
        public bool? IsPaid { get; set; }

        // Czas utworzenia zapisu wypożyczenia
        public DateTime CreationTime { get; set; }

        // Czas ostatniej aktualizacji zapisu
        public DateTime UpdateTime { get; set; }

        // Powiązania z użytkownikiem i filmem (nie będą mapowane w UWP, ale mogą być użyteczne)
        public User User { get; set; }
        public Movie Movie { get; set; }
    }
}
