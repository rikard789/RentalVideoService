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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Frontend
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogInPage : Page
    {
        public LogInPage()
        {
            this.InitializeComponent();
        }
        private void OnLogInClick(object sender, RoutedEventArgs e)
        {
            // Navigate to SignInPage
            this.Frame.Navigate(typeof(UserRentalHistoryPage));
        }

        private void OnSignInClick(object sender, RoutedEventArgs e)
        {
            // Navigate to RegisterPage (if you have one)
            this.Frame.Navigate(typeof(SignInPage));
        }
    }
}
