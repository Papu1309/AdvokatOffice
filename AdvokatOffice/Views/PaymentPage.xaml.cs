using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AdvokatOffice.Connect;
using AdvokatOffice.Helpers;

namespace AdvokatOffice.Views
{
    public partial class PaymentPage : Page
    {
        private Services _selectedService;

        public PaymentPage(Services service)
        {
            InitializeComponent();
            _selectedService = service;
            txtServiceName.Text = service.Name;
            txtPrice.Text = service.Price.ToString("C");

        }

        private void PaymentMethod_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPaymentMethod.SelectedItem is ComboBoxItem selected)
            {
                bool isCard = selected.Tag.ToString() == "Card";
                if (cardPanel != null)
                    cardPanel.Visibility = isCard ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string method = ((ComboBoxItem)cmbPaymentMethod.SelectedItem).Tag.ToString();

                if (method == "Card")
                {
                    string cardNumber = txtCardNumber.Text.Replace(" ", "");
                    if (cardNumber.Length != 16 || !cardNumber.All(char.IsDigit))
                    {
                        MessageBox.Show("Номер карты должен содержать 16 цифр", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string expiry = txtExpiry.Text;
                    if (!Regex.IsMatch(expiry, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                    {
                        MessageBox.Show("Введите срок в формате ММ/ГГ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (txtCvc.Text.Length != 3 || !txtCvc.Text.All(char.IsDigit))
                    {
                        MessageBox.Show("CVC должен содержать 3 цифры", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var order = new Orders
                {
                    UserId = Session.CurrentUser.Id,
                    ServiceId = _selectedService.Id,
                    OrderDate = DateTime.Now,
                    PaymentMethod = method,
                    PaymentStatus = "Оплачено",
                    PickupAddress = "г. Казань, ул. Татарстан, д. 10, офис 5",
                    PickupTime = DateTime.Now.AddHours(1),
                    Status = "Ожидает",
                    Notes = $"Оплата {method}"
                };

                Connection.entities.Orders.Add(order);
                Connection.entities.SaveChanges();

                MessageBox.Show($"✅ Заказ успешно оплачен!\n\n📍 Адрес получения:\n{order.PickupAddress}\n\n⏰ Время получения:\n{order.PickupTime:dd.MM.yyyy HH:mm}\n\n📋 Номер заказа: {order.Id}",
                                "Успешная оплата", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new UserMainPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оплате: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}