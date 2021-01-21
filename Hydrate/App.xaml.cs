using Hydrate.Views.Main;
using System;
using System.Drawing;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Hydrate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Forms.NotifyIcon NotifyIcon;

        public App()
        {
            NotifyIcon = new Forms.NotifyIcon();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();

            NotifyIcon.Icon = new Icon("Resources/omegarts_white.ico");
            NotifyIcon.Text = "Hydrate";
            NotifyIcon.Visible = true;
            NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            NotifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            NotifyIcon.ContextMenuStrip.Items.Add("Exit", Image.FromFile("Resources/Icons/close_black.png"), OnExitClicked);
            base.OnStartup(e);
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            NotifyIcon.Dispose();
            Shutdown();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Show();
            MainWindow.Activate();
        }
    }
}