using Hydrate.Views.Noitifications;
using System;
using System.Globalization;
using System.Windows.Threading;

namespace Hydrate.Models
{
    internal class Schedule
    {
        private readonly int Goal;

        public int NeedToDrink { get; }
        public ManipulateList ManipulateList { get; }
        public double RemainingTime { get; }
        public DrinkingListItem LatestItem { get; }
        public double TimeRemainig { get; set; }
        public double NextDrink { get; private set; }

        private readonly DateTime SleepTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23.59.00", "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture);
        private DispatcherTimer Timer;
        private DispatcherTimer TimerForCheck;

        public Schedule(double goal, int currentDrink, ManipulateList manipulateList)
        {
            Goal = (int)(goal * 1000);
            NeedToDrink = Goal - currentDrink;
            ManipulateList = manipulateList;

            if (NeedToDrink <= 0)
            {
                NeedToDrink = 0;
            }

            if (manipulateList.DrinkingList.Count != 0)
            {
                LatestItem = manipulateList.DrinkingList[0];
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
            TimeRemainig = timeInterval - timeGap.TotalMinutes;

            if (TimeRemainig < 0)
            {
                Notify(0);
            }
            else
            {
                Notify(TimeRemainig);
            }
        }

        private void Notify(double time)
        {
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(time)
            };

            Timer.Tick += new EventHandler(Timer_Elapsed);
            Timer.Start();
            Recheck(time + 15);
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            Timer.Stop();
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

        private void Recheck(double time)
        {
            TimerForCheck = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(time)
            };

            TimerForCheck.Tick += new EventHandler(TimerCheck_Elapsed);
            TimerForCheck.Start();
        }

        private void TimerCheck_Elapsed(object sender, EventArgs e)
        {
            TimerForCheck.Stop();
            var totalDrank = 0;
            foreach (var item in ManipulateList.DrinkingList)
            {
                totalDrank += item.QuantityDrank;
            }
            _ = new Schedule(Goal, totalDrank, ManipulateList);
        }
    }
}