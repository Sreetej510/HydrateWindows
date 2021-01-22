using Hydrate.ViewModels;
using System.Windows;

namespace Hydrate.Views.Noitifications
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window
    {
        private NotificationViewModel _binding;

        public Notification(int needToDrink)
        {
            InitializeComponent();
            _binding = new NotificationViewModel(needToDrink);
            DataContext = _binding;
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
            _binding.Player.Stop();
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _binding.Player.Stop();
            Close();
        }
    }
}