using System;
using System.Collections.Generic;

namespace Frontend.Models
{
    public class User
    {
        // Identyfikator użytkownika (przechowywany w bazie danych serwera)
        public int UserId { get; set; }

        // Nazwa użytkownika
        public string Username { get; set; }

        // Hasło użytkownika
        public string Password { get; set; }

        // Rola użytkownika ('User' lub 'Employee')
        public string Role { get; set; }

        // Czas utworzenia konta
        public DateTime CreationTime { get; set; }

        // Czas ostatniej aktualizacji konta
        public DateTime UpdateTime { get; set; }

        // Kolekcja wypożyczeń powiązana z użytkownikiem
        public List<Rental> Rentals { get; set; } // A user can have many rentals
    }
}
