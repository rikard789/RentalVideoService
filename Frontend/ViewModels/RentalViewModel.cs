using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Frontend.ViewModels
{
    internal class RentalViewModel
    {
        private readonly RentalService _rentalService;
        public List<Rental> Rentals { get; private set; }

        public RentalViewModel(string jwtToken)
        {
            _rentalService = new RentalService(jwtToken);
            Rentals = new List<Rental>();
        }

        public async Task LoadRentalsAsync(string userId)
        {
            Rentals = await _rentalService.GetUserRentalsAsync(userId);
        }
    }
}
