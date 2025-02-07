using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Newtonsoft.Json;
using Windows.UI.Xaml.Data; 

namespace Frontend
{
    public sealed partial class UserRentalHistoryPage : Page
    {
        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };

            return new HttpClient(handler) { BaseAddress = new Uri("https://127.0.0.1:7137/") };
        }

        private readonly HttpClient _httpClient = CreateHttpClient();

        public UserRentalHistoryPage()
        {
            this.InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("UserId", out var userId))
            {
                WelcomeText.Text = $"Witaj, użytkowniku ID: {userId}!";
                LoadRentalHistory(Convert.ToInt32(userId)); 
            }
            else
            {
                WelcomeText.Text = "Witaj!";
            }
        }

        private async void LoadRentalHistory(int userId)
        {
            try
            {
                var token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;

                if (string.IsNullOrEmpty(token))
                {
                    await ShowMessage("Nie znaleziono tokenu. Proszę zalogować się ponownie.");
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        
                string url = $"api/Rental/allUserRentals/{userId}"; 
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var rentals = JsonConvert.DeserializeObject<List<Rental>>(responseBody);

                    if (rentals == null || rentals.Count == 0 )
                    {
                    
                        await ShowMessage("Brak historii wypożyczeń.");
                    }
                    else
                    {
                
                        RentalsListView.ItemsSource = rentals;
                    }
                }
                else
                {
                 
                    await ShowMessage($"Nie udało się pobrać historii wypożyczeń.\nZapytanie: {url}");
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Błąd podczas ładowania historii wypożyczeń.";
                await ShowMessage(errorMessage);
            }
        }

        private void OnRentOnlineClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserDigitalRentalPage));
        }

        private void OnRentalHistoryClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserRentalHistoryPage));
        }

        private void OnLogOutClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LogInPage));
        }



        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        // Klasa reprezentująca dane wypożyczenia
        public class Rental
        {
            public int RentalId { get; set; }
            public int UserId { get; set; }
            public int MovieId { get; set; }
            public DateTime RentalDate { get; set; }
            public DateTime ReturnDate { get; set; }
            public bool IsDamaged { get; set; }
            public decimal Penalty { get; set; }
            public bool IsPaid { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public User User { get; set; }
            public Movie Movie { get; set; }
        }

        public class User
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public List<string> Rentals { get; set; }
        }

        public class Movie
        {
            public int MovieId { get; set; }
            public string Title { get; set; }
            public string Genres { get; set; }
            public string Type { get; set; }
            public string Image { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public List<string> Rentals { get; set; }
        }
    }
}
