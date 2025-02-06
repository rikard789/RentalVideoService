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
    public sealed partial class MovieListPage : Page
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

        public MovieListPage()
        {
            this.InitializeComponent();
            LoadMovies();
        }

        private async void LoadMovies()
        {
            var token = ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;

            if (string.IsNullOrEmpty(token))
            {
                await ShowMessage("Brak tokena, użytkownik nie jest zalogowany.");
                return;
            }

            try
            {
                var movies = await GetMoviesFromBackend(token);
                MoviesListView.ItemsSource = movies;
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd podczas ładowania filmów: " + ex.Message);
            }
        }

        private async Task<List<Movie>> GetMoviesFromBackend(string token)
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

        // Wyszukiwanie po nazwie
        private async void SearchByNameButton_Click(object sender, RoutedEventArgs e)
        {
            var token = ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;
            string name = SearchByNameTextBox.Text;

            if (string.IsNullOrEmpty(token))
            {
                await ShowMessage("Brak tokena, użytkownik nie jest zalogowany.");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                await ShowMessage("Wpisz nazwę filmu.");
                return;
            }

            try
            {
                var movies = await SearchMoviesByName(token, name);
                MoviesListView.ItemsSource = movies;
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd podczas wyszukiwania: " + ex.Message);
            }
        }

        private async Task<List<Movie>> SearchMoviesByName(string token, string name)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Movies/searchByName/{name}");
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
                throw new Exception("Nie udało się znaleźć filmów.");
            }
        }

        // Wyszukiwanie po ID
        private async void SearchByIdButton_Click(object sender, RoutedEventArgs e)
        {
            var token = ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;
            string id = SearchByIdTextBox.Text;

            if (string.IsNullOrEmpty(token))
            {
                await ShowMessage("Brak tokena, użytkownik nie jest zalogowany.");
                return;
            }

            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int movieId))
            {
                await ShowMessage("Wpisz poprawne ID filmu.");
                return;
            }

            try
            {
                var movie = await SearchMovieById(token, movieId);
                MoviesListView.ItemsSource = new List<Movie> { movie };
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd podczas wyszukiwania: " + ex.Message);
            }
        }

        private async Task<Movie> SearchMovieById(string token, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Movies/searchById/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var movie = JsonConvert.DeserializeObject<Movie>(responseBody);
                return movie;
            }
            else
            {
                throw new Exception("Nie udało się znaleźć filmu.");
            }
        }

        private async Task ShowMessage(string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }

        private void OnAddProductClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EmployeeProductIntroductionPage));
        }

        private void OnLogOutClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LogInPage));
        }

    }

    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genres { get; set; }
        public string Type { get; set; }
        public string RentalHistory { get; set; }
    }
}