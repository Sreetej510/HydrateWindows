﻿using Hydrate.Views.Noitifications;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Hydrate.Models
{
    internal class Schedule
    {
        private readonly int Goal;

        private int NeedToDrink;
        private readonly ManipulateList ManipulateList;

        private double RemainingTime;
        private DrinkingListItem LatestItem;
        private double TimeRemaining;
        private double NextDrink;

        private readonly DateTime SleepTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23.59.00", "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture);
        private readonly DispatcherTimer TimerForCheck;

        public Schedule(double goal, ManipulateList manipulateList)
        {
            Goal = (int)(goal * 1000);
            ManipulateList = manipulateList;
            TimerForCheck = new DispatcherTimer();
            Test();
        }

        public void Test()
        {
            var totalDrank = 0;
            foreach (var item in ManipulateList.DrinkingList)
            {
                totalDrank += item.QuantityDrank;
            }
            NeedToDrink = Goal - totalDrank;

            if (NeedToDrink <= 0)
            {
                NeedToDrink = 0;
            }

            if (ManipulateList.DrinkingList.Count != 0)
            {
                LatestItem = ManipulateList.DrinkingList[0];
            }
            else
            {
                DateTime tempTime;
                if (DateTime.Now.Hour >= 6)
                {
                    tempTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 06.00.00", "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture);
                }
                else
                {
                    tempTime = DateTime.Now;
                }

                LatestItem = new DrinkingListItem(true, 250) { DrankTime = tempTime };
            }

            RemainingTime = (SleepTime - DateTime.Now).TotalMinutes;

            TimerSet();
        }

        private void TimerSet()
        {
            var nextDrink_min = NeedToDrink / RemainingTime;
            var lastDrankQuantity = LatestItem.QuantityDrank;
            var lastDrankTime = LatestItem.DrankTime;
            var hasEaten = LatestItem.Eaten;
            double timeInterval;
            if (nextDrink_min > 0)
            {
                if (lastDrankQuantity >= 250)
                {
                    timeInterval = 45;
                }
                else if (lastDrankQuantity <= 200)
                {
                    timeInterval = 30;
                }
                else
                {
                    timeInterval = (((NeedToDrink - 200) * 50) / 15) + 30;
                }
            }
            else
            {
                return;
            }
            NextDrink = nextDrink_min * timeInterval;
            if (NextDrink > 300)
            {
                NextDrink = 300;
            }
            else if (NextDrink < 150)
            {
                NextDrink = 150;
            }

            var timeGap = DateTime.Now - lastDrankTime;
            if (hasEaten)
            {
                timeInterval = 60;
            }
            TimeRemaining = timeInterval - timeGap.TotalMinutes;

            if (TimeRemaining < 0)
            {
                TimeRemaining = 0;
            }

            Recheck(TimeRemaining);
        }

        private void Recheck(double time)
        {
            if (time == 0)
            {
                Notify();
                time += 15;
            }

            TimerForCheck.Interval = TimeSpan.FromMinutes(time);

            TimerForCheck.Tick += new EventHandler(TimerCheck_Elapsed);

            TimerForCheck.Start();
        }

        private void TimerCheck_Elapsed(object sender, EventArgs e)
        {
            TimerForCheck.Stop();
            Test();
        }

        private void Notify()
        {
            var window = new Notification((int)NextDrink);
            try
            {
                window.Show();
                window.Activate();
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}