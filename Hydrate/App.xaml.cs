using Hydrate.Models;
using Hydrate.Views.Main;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
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
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();

            NotifyIcon.Icon = new Icon("Resources/hydrate.ico");
            NotifyIcon.Text = "Hydrate";
            NotifyIcon.Visible = true;
            NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;

            NotifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            NotifyIcon.ContextMenuStrip.Items.Add("Snooze for hour", Image.FromFile("Resources/Icons/snooze.png"), OnSnoozeClicked);
            NotifyIcon.ContextMenuStrip.Items.Add("Exit", Image.FromFile("Resources/Icons/close_black.png"), OnExitClicked);
            base.OnStartup(e);
        }


        private void OnExitClicked(object sender, EventArgs e)
        {
            NotifyIcon.Dispose();
            Shutdown();
        }

        private void OnSnoozeClicked(object sender, EventArgs e)
        {
            var schedule = Schedule.GetSchedule();
            schedule.Recheck(60);
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }
    }
}