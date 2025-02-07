using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

namespace Frontend
{
    public sealed partial class RentMoviePage : Page
    {
        private readonly HttpClient _httpClient;

        public RentMoviePage()
        {
            this.InitializeComponent();
            _httpClient = CreateHttpClient();
        }

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };

            return new HttpClient(handler) { BaseAddress = new Uri("https://127.0.0.1:7137/") };
        }

        private async void OnRentClick(object sender, RoutedEventArgs e)
        {
            string userId = UserIdTextBox.Text;
            string movieId = MovieIdTextBox.Text;

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(movieId))
            {
                await ShowMessage("Proszę podać User ID i Movie ID.");
                return;
            }

            bool isSuccess = await RentMovie(userId, movieId);
            if (isSuccess)
            {
                await ShowMessage("Film został pomyślnie wypożyczony.");
                this.Frame.Navigate(typeof(UserDigitalRentalPage));
            }
            else
            {
                await ShowMessage("Wystąpił błąd podczas wypożyczania filmu.");
            }
        }

        private async Task<bool> RentMovie(string userId, string movieId)
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (!localSettings.Values.ContainsKey("JwtToken"))
                {
                    await ShowMessage("Nie jesteś zalogowany. Zaloguj się, aby wypożyczyć film.");
                    return false;
                }

                string jwtToken = localSettings.Values["JwtToken"].ToString();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                string requestUri = $"/api/Rental/addMovie/{userId}/{movieId}";
                HttpResponseMessage response = await _httpClient.PostAsync(requestUri, null);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    await ShowMessage($"Błąd: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ShowMessage($"Błąd: {ex.Message}");
                return false;
            }
        }

        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserDigitalRentalPage));
        }


    }
}
