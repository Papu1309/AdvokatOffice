using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AdvokatOffice.Connect;
using AdvokatOffice.Helpers;

namespace AdvokatOffice.Views
{
    public partial class UserOrdersPage : Page
    {
        public UserOrdersPage()
        {
            InitializeComponent();
            LoadOrders();

        }

        private void LoadOrders()
        {
            var orders = Connection.entities.Orders
                .Where(o => o.UserId == Session.CurrentUser.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            dgOrders.ItemsSource = orders;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserMainPage());
        }
    }
}