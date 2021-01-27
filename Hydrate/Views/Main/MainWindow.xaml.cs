using Hydrate.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hydrate.Views.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _bindingContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _bindingContext = new MainWindowViewModel();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Minimize_Clicked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Close_Clicked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            Hide();
        }

        protected override void OnActivated(EventArgs e)
        {
            _bindingContext.UpdateTotalDrank();
            Task.Run(() => _bindingContext.PopulateList.ListRefresh());
            base.OnActivated(e);
        }
    }
}