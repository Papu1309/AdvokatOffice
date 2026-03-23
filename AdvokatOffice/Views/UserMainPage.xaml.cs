using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AdvokatOffice.Connect;
using AdvokatOffice.Helpers;

namespace AdvokatOffice.Views
{
    public partial class UserMainPage : Page
    {
        public UserMainPage()
        {
            InitializeComponent();
            LoadServices();
            DataContext = Session.CurrentUser;

        }

        private void LoadServices()
        {
            dgServices.ItemsSource = Connection.entities.Services.ToList();
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgServices.SelectedItem as Services;
            if (selected == null)
            {
                MessageBox.Show("Выберите услугу для оплаты", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NavigationService.Navigate(new PaymentPage(selected));
        }

        private void MyOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserOrdersPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            NavigationService.Navigate(new LoginPage());
        }
    }
}