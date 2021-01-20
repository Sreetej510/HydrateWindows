using System;

namespace Hydrate.ViewModels
{
    internal class NotificationViewModel
    {
        public int NeedToDrink { get; }
        public DateTime Time { get; }

        public double left1 { get; set; }
        public double left2 { get; set; }

        public NotificationViewModel(int needToDrink)
        {
            NeedToDrink = needToDrink;
            Time = DateTime.Now;
            left1 = System.Windows.SystemParameters.PrimaryScreenWidth - 200;
            left2 = System.Windows.SystemParameters.PrimaryScreenWidth - 520;
        }
    }
}