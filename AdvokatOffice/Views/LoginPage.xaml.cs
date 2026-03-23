using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AdvokatOffice.Connect;
using AdvokatOffice.Helpers;

namespace AdvokatOffice.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            var user = Connection.entities.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                Session.CurrentUser = user;
                if (user.Roles.Name == "Администратор")
                    NavigationService.Navigate(new AdminMainPage());
                else
                    NavigationService.Navigate(new UserMainPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}