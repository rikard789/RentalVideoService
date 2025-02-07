using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.Storage;

namespace Frontend
{
    public sealed partial class UserDigitalRentalPage : Page
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

        public UserDigitalRentalPage()
        {
            this.InitializeComponent();
            LoadDigitalMovies();
        }

        private async void LoadDigitalMovies()
        {
            var token = ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;

            if (string.IsNullOrEmpty(token))
            {
                await ShowMessage("Brak tokena, użytkownik nie jest zalogowany.");
                return;
            }

            try
            {
                var movies = await GetDigitalMoviesFromBackend(token);
                DigitalMoviesListView.ItemsSource = movies;
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd podczas ładowania filmów: " + ex.Message);
            }
        }

        private async Task<List<Movie>> GetDigitalMoviesFromBackend(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Movies/getAllMovies");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var movies = JsonConvert.DeserializeObject<List<Movie>>(responseBody);
                return movies;
            }
            else
            {
                throw new Exception("Nie udało się pobrać danych z serwera.");
            }
        }

        private async Task ShowMessage(string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }

        private void RentMovieButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                int movieId = (int)button.Tag;
                // Przekierowanie do strony wypożyczenia filmu
               // this.Frame.Navigate(typeof(RentMoviePage), movieId);
            }
        }

        private void OnLogOutClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LogInPage));
        }
    }


}
