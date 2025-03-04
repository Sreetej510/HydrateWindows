﻿using System;
using System.Media;
using System.Windows;

namespace Hydrate.ViewModels
{
    internal class NotificationViewModel
    {
        public int NeedToDrink { get; }
        public DateTime Time { get; }

        public double Left1 { get; set; }
        public double Left2 { get; set; }
        public double Top { get; set; }
        public SoundPlayer Player { get; }

        public NotificationViewModel(int needToDrink)
        {
            NeedToDrink = needToDrink;
            Time = DateTime.Now;

            Left1 = SystemParameters.PrimaryScreenWidth - 200;
            Left2 = SystemParameters.PrimaryScreenWidth - 520;

            var count = Application.Current.Windows.Count;

            if (count > 5)
            {
                Application.Current.Windows[count - 1].Close();
            }

            Top = (count - 2) * 120 + count * 10;
            Player = new SoundPlayer(@"Resources/Sounds/LlamaBell.wav");
            Player.Play();
        }
    }
}