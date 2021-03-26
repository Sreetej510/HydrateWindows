using Hydrate.ViewModels;
using System.Windows;

namespace Hydrate.Views.Noitifications
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window
    {
        private readonly NotificationViewModel _binding;

        public Notification(int needToDrink)
        {
            InitializeComponent();
            _binding = new NotificationViewModel(needToDrink);
            DataContext = _binding;
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
            _binding.Player.Stop();
            closeAll();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            _binding.Player.Stop();
            closeAll();
        }

        private void closeAll()
        {
            var allWindows = Application.Current.Windows;
            foreach (Window window in allWindows)
            {
                if (window.GetType().Name != "MainWindow")
                {
                    window.Close();
                };
            }
        }
    }
}