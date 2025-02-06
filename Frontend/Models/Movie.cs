using System;
using System.Collections.Generic;

namespace Frontend.Models
{
    public class Movie
    {
        // Identyfikator filmu
        public int MovieId { get; set; }

        // Tytuł filmu
        public string Title { get; set; }

        // Gatunki filmu (możesz użyć jednego ciągu znaków lub listy)
        public string Genres { get; set; }

        // Typ filmu ('DVD' lub 'Electronic')
        public string Type { get; set; }

        // Ścieżka do obrazu (może być null)
        public string? Image { get; set; }

        // Data utworzenia filmu (może być null)
        public DateTime? CreationTime { get; set; }

        // Data ostatniej aktualizacji (może być null)
        public DateTime? UpdateTime { get; set; }

        // Kolekcja wypożyczeń, w których pojawia się film (opcjonalnie)
        public ICollection<Rental>? Rentals { get; set; } // A movie can appear in multiple rentals
    }
}
