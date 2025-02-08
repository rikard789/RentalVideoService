using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage;

namespace Frontend
{
    public sealed partial class EmployeeProductHandlingPage : Page
    {
        private readonly HttpClient _httpClient;

        public EmployeeProductHandlingPage()
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

        private async void OnSetPenaltyClick(object sender, RoutedEventArgs e)
        {
            string rentalId = RentalIdTextBox.Text;
            bool isDamaged = IsDamagedComboBox.SelectedIndex == 0; 
            string penaltyText = PenaltyTextBox.Text;

            if (string.IsNullOrWhiteSpace(rentalId) || string.IsNullOrWhiteSpace(penaltyText))
            {
                await ShowMessage("Proszę podać ID wypożyczenia i wartość kary.");
                return;
            }

            if (!decimal.TryParse(penaltyText, out decimal penalty))
            {
                await ShowMessage("Wartość kary musi być liczbą.");
                return;
            }

            bool isSuccess = await SetPenalty(rentalId, isDamaged, penalty);
            if (isSuccess)
            {
                await ShowMessage("Kara została pomyślnie naliczona.");
            }
            else
            {
                await ShowMessage("Wystąpił błąd podczas naliczania kary.");
            }
        }

        private async Task<bool> SetPenalty(string  rentalId, bool isDamaged, decimal penalty)
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (!localSettings.Values.ContainsKey("JwtToken"))
                {
                    await ShowMessage("Nie jesteś zalogowany. Zaloguj się, aby wykonać tę operację.");
                    return false;
                }

                string jwtToken = localSettings.Values["JwtToken"].ToString();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                string requestUri = $"/api/Rental/set-penalty{rentalId}/{isDamaged}/{penalty}";
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
            var dialog = new ContentDialog
            {
                Title = "Informacja",
                Content = message,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close
            };

            await dialog.ShowAsync();
        }

        private void OnAddProductClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EmployeeProductIntroductionPage));
        }

        private void OnViewMoviesClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MovieListPage));
        }

        private void OnLogOutClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LogInPage));
        }
    }
}