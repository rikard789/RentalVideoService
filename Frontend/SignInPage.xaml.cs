using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Newtonsoft.Json;


namespace Frontend
{
    public sealed partial class SignInPage : Page
    {

        public static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };

            return new HttpClient(handler) { BaseAddress = new Uri("https://127.0.0.1:7137/") };
        }

        private readonly HttpClient _httpClient = CreateHttpClient();

        public SignInPage()
        {
            this.InitializeComponent();
        }

        private async void OnSignInClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string userType = ((ComboBoxItem)UserTypeComboBox.SelectedItem)?.Content.ToString();

            if (userType == "Użytkownik")
            {
                userType = "user";
            }
            else if (userType == "Pracownik")
            {
                userType = "admin";
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(userType))
            {
                await ShowMessage("Wszystkie pola są wymagane.");
                return;
            }

            var isSuccess = await RegisterUser(username, password, userType);
            if (isSuccess)
            {
                await ShowMessage("Rejestracja zakończona sukcesem! Możesz się teraz zalogować.");
                this.Frame.Navigate(typeof(LogInPage)); 
            }
        }

        private async Task<bool> RegisterUser(string username, string password, string userType)
        {
            try
            {
                var requestBody = new
                {
                    username = username,
                    password = password,
                    role = userType,
                    rentals = new List<string>() 
                };


                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("api/Users/createUser", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    await ShowMessage("Błąd rejestracji: " + errorResponse);
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd połączenia: " + ex.Message);
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



        private void OnLogInClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LogInPage));
        }
    }

}
