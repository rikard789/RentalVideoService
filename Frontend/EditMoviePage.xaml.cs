using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.Storage;
using Windows.UI.Popups;

namespace Frontend
{
    public sealed partial class EditMoviePage : Page
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
        private int _movieId;

        public EditMoviePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _movieId = (int)e.Parameter;
            LoadMovieDetails(_movieId);
        }

        private async void LoadMovieDetails(int movieId)
        {
            var token = ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;

            if (string.IsNullOrEmpty(token))
            {
                await ShowMessage("Brak tokena, użytkownik nie jest zalogowany.");
                return;
            }

            try
            {
                var movie = await GetMovieById(movieId, token);
                TitleTextBox.Text = movie.Title;
                GenresTextBox.Text = movie.Genres;
                TypeTextBox.Text = movie.Type;
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd podczas ładowania danych filmu: " + ex.Message);
            }
        }

        private async Task<Movie> GetMovieById(int movieId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Movies/searchById/{movieId}");
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
                throw new Exception("Nie udało się pobrać danych filmu.");
            }
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            var token = ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;

            if (string.IsNullOrEmpty(token))
            {
                await ShowMessage("Brak tokena, użytkownik nie jest zalogowany.");
                return;
            }

            var updatedMovie = new
            {
                title = TitleTextBox.Text,
                genres = GenresTextBox.Text,
                type = TypeTextBox.Text
            };

            try
            {
                var isSuccess = await UpdateMovie(_movieId, updatedMovie, token);
                if (isSuccess)
                {
                    await ShowMessage("Film został zaktualizowany pomyślnie!");
                    this.Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd podczas aktualizacji filmu: " + ex.Message);
            }
        }

        private async Task<bool> UpdateMovie(int movieId, object updatedMovie, string token)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(updatedMovie);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Put, $"api/Movies/updateMovie/{movieId}")
                {
                    Content = content
                };
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas aktualizacji filmu: " + ex.Message);
            }
        }

        private void OnViewMoviesClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MovieListPage));
        }


        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }
    }
}