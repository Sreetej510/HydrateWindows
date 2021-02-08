using Hydrate.Views.Noitifications;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace Hydrate.Models
{
    internal class Schedule
    {
        private readonly static Schedule _instance = new Schedule();

        public static Schedule GetSchedule()
        {
            return _instance;
        }

        private static int Goal;

        private int NeedToDrink;
        private static ManipulateList ManipulateList;

        private double RemainingTime;
        private DrinkingListItem LatestItem;
        private double TimeRemaining;
        private double NextDrink;
        private readonly DateTime SleepTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23.59.00", "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture);
        private readonly DispatcherTimer TimerForCheck;

        private Schedule()
        {
            ManipulateList = ManipulateList.GetManipulateList();
            Goal = ManipulateList.Goal * 1000;
            TimerForCheck = new DispatcherTimer();
            TimerForCheck.Tick += new EventHandler(OnTick);
        }

        public void StartSchedule(bool refresh = false)
        {
            if (refresh)
            {
                ManipulateList.ListRefresh();
            }

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
                bool tempEaten = true;
                DateTime tempTime;
                if (DateTime.Now.Hour >= 6)
                {
                    tempTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 06.00.00", "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture);
                    tempEaten = false;
                }
                else
                {
                    tempTime = DateTime.Now;
                }

                LatestItem = new DrinkingListItem(tempEaten, 250) { DrankTime = tempTime };
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
                if (lastDrankQuantity >= 300)
                {
                    timeInterval = 50;
                }
                else if (lastDrankQuantity <= 200)
                {
                    timeInterval = 30;
                }
                else
                {
                    timeInterval = ((lastDrankQuantity - 200) / 5) + 30;
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
            else if (NextDrink < 100)
            {
                NextDrink = 100;
            }

            var timeGap = DateTime.Now - lastDrankTime;
            if (hasEaten)
            {
                timeInterval = 75;
            }

            TimeRemaining = timeInterval - timeGap.TotalMinutes;
            Recheck(TimeRemaining);
        }

        private void Recheck(double time)
        {
            if (time < 0)
            {
                Notify();
                time = 15;
            }
            ManipulateList.NextDrinkTime = DateTime.Now.AddMinutes(time);

            TimerForCheck.Interval = TimeSpan.FromMinutes(time);

            TimerForCheck.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            TimerForCheck.Stop();
            StartSchedule(true);
        }

        private void Notify()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var window = new Notification((int)NextDrink);
                try
                {
                    window.Show();
                }
                catch (Exception)
                {
                    return;
                }
            }));
        }
    }
}