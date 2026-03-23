using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdvokatOffice.Connect;
using AdvokatOffice.Helpers;

namespace AdvokatOffice.Views
{
    public partial class AdminMainPage : Page
    {
        public AdminMainPage()
        {
            InitializeComponent();
            LoadServices();
            LoadUsers();

        }

        private void LoadServices()
        {
            try
            {
                dgServices.ItemsSource = Connection.entities.Services.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUsers()
        {
            try
            {
                dgUsers.ItemsSource = Connection.entities.Users.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrders()
        {
            try
            {
                var orders = Connection.entities.Orders
                    .Include("Users")
                    .Include("Services")
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                dgOrders.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.DataContext is Orders order)
            {
                foreach (ComboBoxItem item in comboBox.Items)
                {
                    if (item.Tag.ToString() == order.Status)
                    {
                        comboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ServiceEditWindow();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var newService = new Services
                    {
                        Name = dialog.ServiceName,
                        Description = dialog.ServiceDescription,
                        Price = dialog.ServicePrice,
                        Duration = dialog.ServiceDuration
                    };
                    Connection.entities.Services.Add(newService);
                    Connection.entities.SaveChanges();
                    LoadServices();
                    MessageBox.Show("Услуга успешно добавлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgServices.SelectedItem as Services;
            if (selected == null)
            {
                MessageBox.Show("Выберите услугу для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dialog = new ServiceEditWindow(selected);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    selected.Name = dialog.ServiceName;
                    selected.Description = dialog.ServiceDescription;
                    selected.Price = dialog.ServicePrice;
                    selected.Duration = dialog.ServiceDuration;
                    Connection.entities.SaveChanges();
                    LoadServices();
                    MessageBox.Show("Услуга успешно обновлена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgServices.SelectedItem as Services;
            if (selected == null)
            {
                MessageBox.Show("Выберите услугу для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить услугу \"{selected.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var relatedOrders = Connection.entities.Orders.Where(o => o.ServiceId == selected.Id).ToList();
                    foreach (var order in relatedOrders)
                    {
                        Connection.entities.Orders.Remove(order);
                    }

                    Connection.entities.Services.Remove(selected);
                    Connection.entities.SaveChanges();

                    MessageBox.Show("Услуга успешно удалена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadServices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var orders = dgOrders.ItemsSource as System.Collections.IEnumerable;
                if (orders == null)
                {
                    MessageBox.Show("Нет заказов для сохранения", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int updatedCount = 0;

                foreach (Orders order in orders)
                {
                    var dbOrder = Connection.entities.Orders.Find(order.Id);
                    if (dbOrder != null && dbOrder.Status != order.Status)
                    {
                        dbOrder.Status = order.Status;
                        updatedCount++;
                    }
                }

                if (updatedCount > 0)
                {
                    Connection.entities.SaveChanges();
                    MessageBox.Show($"Статусы заказов сохранены. Обновлено: {updatedCount} заказ(ов)",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrders();
                }
                else
                {
                    MessageBox.Show("Статусы заказов сохранены", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
            MessageBox.Show("Список заказов обновлен", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.Tag as Users;

            if (user != null)
            {
                if (user.Login == "admin")
                {
                    MessageBox.Show("Нельзя удалить главного администратора", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить пользователя {user.Login}?\n\nВсе заказы пользователя также будут удалены!", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var dbUser = Connection.entities.Users.Find(user.Id);
                        if (dbUser != null)
                        {
                            Connection.entities.Users.Remove(dbUser);
                            Connection.entities.SaveChanges();

                            MessageBox.Show("Пользователь успешно удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadUsers();
                            LoadOrders();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            NavigationService.Navigate(new LoginPage());
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tabControl && tabControl.SelectedItem is TabItem selectedTab)
            {
                string header = selectedTab.Header.ToString();

                if (header.Contains("Заказы") && dgOrders != null && dgOrders.ItemsSource == null)
                {
                    LoadOrders();
                }
                else if (header.Contains("Пользователи") && dgUsers != null && dgUsers.ItemsSource == null)
                {
                    LoadUsers();
                }
            }
        }
    }
}