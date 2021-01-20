using Hydrate.ViewModels;
using System.Windows;

namespace Hydrate.Views.Noitifications
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window
    {
        public Notification(int needToDrink)
        {
            InitializeComponent();
            DataContext = new NotificationViewModel(needToDrink);
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }
    }
}