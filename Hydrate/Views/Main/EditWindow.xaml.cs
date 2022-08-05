using Hydrate.Models;
using Hydrate.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Hydrate.Views.Main
{
    /// <summary>
    /// Interaction logic for EditPage.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow(DrinkingListItem editItem)
        {
            InitializeComponent();
            DataContext = new EditWindowViewModel(editItem, window);
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}