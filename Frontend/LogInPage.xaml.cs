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
using System.IdentityModel.Tokens.Jwt;
using Frontend.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Frontend
{
    // SSL cert fix 
    public sealed partial class LogInPage : Page
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

        public LogInPage()
        {
            this.InitializeComponent();
        }

        private async void OnLogInClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await ShowMessage("Proszę podać nazwę użytkownika i hasło.");
                return;
            }

            var isSuccess = await AuthenticateUser(username, password);
            if (isSuccess) {}
        }

        private void OnSignInClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SignInPage));
        }

        private async Task<bool> AuthenticateUser(string username, string password)
        {
            try
            {
                var requestBody = new { username = username, password = password };
                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("api/Authorization/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<TokenResponse>(responseBody);


                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["JwtToken"] = tokenData.Token;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserId"] = tokenData.UserId;

                    //var userId = ExtractUserIdFromToken(tokenData.Token);
                    //if (!string.IsNullOrEmpty(userId))
                    //{
                    //    Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserId"] = userId;
                    //}

                    // Zapisujemy login użytkownika
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["Username"] = username;

                    // Przekierowanie na podstawie roli
                    if (tokenData.Role == "admin")
                    {
                        this.Frame.Navigate(typeof(EmployeeProductHandlingPage));
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(UserRentalHistoryPage));
                    }

                    return true;
                }
                else
                {
                    await ShowMessage("Nieprawidłowa nazwa użytkownika lub hasło.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await ShowMessage("Błąd logowania: " + ex.Message + " " + ex.InnerException?.Message);
                return false;
            }
        }

        //private string ExtractUserIdFromToken(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var jwtToken = handler.ReadJwtToken(token);

        //    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        //    return userIdClaim?.Value;
        //}

        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        public class TokenResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
            public int UserId { get; set; }
        }
    }

}
