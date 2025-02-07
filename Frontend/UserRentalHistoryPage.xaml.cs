using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using Newtonsoft.Json;

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

        public UserRentalHistoryPage()
        {
            this.InitializeComponent();
            LoadUserData();
        }

        private async void LoadUserData()
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("Username", out var username))
            {
                WelcomeText.Text = $"Witaj, użytkowniku {username}!";

            }
            else
            {
                WelcomeText.Text = "Witaj!";
            }
        }

    

        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
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

    }
}
