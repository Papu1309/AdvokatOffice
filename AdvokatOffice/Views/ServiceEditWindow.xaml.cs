using System;
using System.Windows;
using AdvokatOffice.Connect;

namespace AdvokatOffice.Views
{
    public partial class ServiceEditWindow : Window
    {
        public string ServiceName { get; private set; }
        public string ServiceDescription { get; private set; }
        public decimal ServicePrice { get; private set; }
        public int ServiceDuration { get; private set; }

        public ServiceEditWindow(Services service = null)
        {
            InitializeComponent();
            if (service != null)
            {
                txtName.Text = service.Name;
                txtDescription.Text = service.Description;
                txtDuration.Text = service.Duration.ToString();
                txtPrice.Text = service.Price.ToString();
            }
            else
            {
                txtDuration.Text = "60";
            }

        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название услуги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtDuration.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Длительность должна быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ServiceName = txtName.Text.Trim();
            ServiceDescription = txtDescription.Text?.Trim() ?? "";
            ServiceDuration = duration;
            ServicePrice = price;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}