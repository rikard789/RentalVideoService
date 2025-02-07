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
    public sealed partial class EmployeeProductIntroductionPage : Page
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

        public EmployeeProductIntroductionPage()
        {
            this.InitializeComponent();
        }

        private async void OnAddProductClick(object sender, RoutedEventArgs e)
        {
            string movieTitle = MovieTitleTextBox.Text;

            string movieGenre = (MovieTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string movieCategory = (MovieCategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();


            if (string.IsNullOrWhiteSpace(movieTitle) || string.IsNullOrWhiteSpace(movieGenre) || string.IsNullOrWhiteSpace(movieCategory))
            {
                await ShowMessage("Proszę uzupełnić wszystkie dane.");
                return;
            }

            var token = Windows.Storage.ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;

            if (string.IsNullOrEmpty(token))
            {
                await ShowMessage("Brak tokena, użytkownik nie jest zalogowany.");
                return;
            }

            var isSuccess = await AddMovieToBackend(movieTitle, movieGenre, movieCategory, token);
            if (isSuccess)
            {
                await ShowMessage("Film został dodany pomyślnie!");
            }
        }

        private async Task<bool> AddMovieToBackend(string title, string type, string genre, string token)
        {
            try
            {
                var requestBody = new
                {
                    title = title,
                    genres = genre,
                    type = type
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, "api/Movies/createMovie")
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
                    await ShowMessage("Błąd podczas dodawania filmu: " + errorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd: " + ex.Message);
                return false;
            }
        }

        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }


        private void OnHandlingProductsClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EmployeeProductHandlingPage));
        }

        private void OnLogOutClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LogInPage));
        }
    }
}
