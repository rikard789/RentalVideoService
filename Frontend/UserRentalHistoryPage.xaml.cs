using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Frontend.Models;

namespace Frontend
{
    public sealed partial class UserRentalHistoryPage : Page
    {
        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return new HttpClient(handler) { BaseAddress = new Uri("https://127.0.0.1:7137/") };
        }

        private readonly HttpClient _httpClient = CreateHttpClient();
        private List<Rental> _rentalHistory = new List<Rental>();

        public UserRentalHistoryPage()
        {
            this.InitializeComponent();
            LoadUserRentals();
        }

        private async void LoadUserRentals()
        {
            try
            {
                if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("JwtToken"))
                {
                    await ShowMessage("Brak autoryzacji. Zaloguj się ponownie.");
                    this.Frame.Navigate(typeof(LogInPage));
                    return;
                }

                string token = ApplicationData.Current.LocalSettings.Values["JwtToken"] as string;

                int userId = (int)ApplicationData.Current.LocalSettings.Values["UserId"];

                _rentalHistory = await GetUserRentals(token, userId);

                RentalListView.ItemsSource = _rentalHistory;
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd: " + ex.Message);
            }
        }

        private async Task<List<Rental>> GetUserRentals(string token, int userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Rental/allUserRentals/{userId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                try
                {
                    var rentals = JsonConvert.DeserializeObject<List<Rental>>(responseBody);

                    foreach (var rental in rentals)
                    {
                        rental.Penalty ??= 0;
                    }

                    return rentals;
                }
                catch (JsonException ex)
                {
                    await ShowMessage("Błąd parsowania JSON: " + ex.Message);
                    return new List<Rental>();
                }
            }
            else
            {
                await ShowMessage($"Błąd serwera ({response.StatusCode}): Nie udało się pobrać historii wypożyczeń.");
                return new List<Rental>();
            }
        }

        private async Task ShowMessage(string message)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(message);
            await dialog.ShowAsync();
        }

        private void OnRentPhysicalClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserPhysicalRentalPage));
        }

        private void OnRentOnlineClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserDigitalRentalPage));
        }

        private void OnLogOutClick(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values.Remove("JwtToken");
            ApplicationData.Current.LocalSettings.Values.Remove("UserId");
            this.Frame.Navigate(typeof(LogInPage));
        }
    }
}
