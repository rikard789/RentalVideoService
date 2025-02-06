//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;

//namespace Frontend.Services
//{
//    internal class RentalService
//    {
//        private const string ApiBaseUrl = "https://your-api-url.com"; // Zamień na właściwy URL
//        private readonly string _jwtToken;

//        public RentalService(string jwtToken)
//        {
//            _jwtToken = jwtToken;
//        }

//        public async Task<List<Rental>> GetUserRentalsAsync(string userId)
//        {
//            string url = $"{ApiBaseUrl}/api/Rental/allUserRentals/{userId}";

//            using (var client = new HttpClient())
//            {
//                // Dodanie tokenu JWT do nagłówków
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

//                try
//                {
//                    // Wykonanie zapytania do API
//                    var response = await client.GetStringAsync(url);
//                    var rentals = JsonConvert.DeserializeObject<List<Rental>>(response);
//                    return rentals;
//                }
//                catch (Exception ex)
//                {
//                    // Obsługa błędów
//                    throw new Exception("Błąd pobierania danych z serwera: " + ex.Message);
//                }
//            }
//        }
//    }
//}
