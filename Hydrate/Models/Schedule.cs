using Hydrate.Views.Noitifications;
using System;
using System.Globalization;
using System.Windows.Threading;

namespace Hydrate.Models
{
    internal class Schedule
    {
        private int Goal;

        public int NeedToDrink { get; }
        public ManipulateList ManipulateList { get; }
        public double RemainingTime { get; }
        public DrinkingListItem LatestItem { get; }
        public double Timer { get; set; }
        public double NextDrink { get; private set; }

        private DateTime SleepTime;
        private DispatcherTimer timer;
        private DispatcherTimer timerCheck;

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
                var tempTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 00.00.00", "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture);
                LatestItem = new DrinkingListItem() { DrankTime = tempTime, QuantityDrank = 250 };
            }

            SleepTime = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23.59.00", "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture);
            RemainingTime = (SleepTime - DateTime.Now).TotalMinutes;

            TimerSet();
        }

        private void TimerSet()
        {
            var nextDrink_min = NeedToDrink / RemainingTime;
            var lastDrankQuantity = LatestItem.QuantityDrank;
            var lastDrankTime = LatestItem.DrankTime;
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

            var timeGap = DateTime.Now - lastDrankTime;
            Timer = timeInterval - timeGap.TotalMinutes;
            if (Timer < 0)
            {
                Notify(0);
            }
            else
            {
                Notify(Timer);
            }
        }

        private void Notify(double time)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(time);
            timer.Tick += new EventHandler(timer_Elapsed);
            timer.Start();
            Recheck(time + 30);
        }

        private void timer_Elapsed(object sender, EventArgs e)
        {
            timer.Stop();
            var window = new Notification((int)NextDrink);
            window.Show();
        }

        private void Recheck(double time)
        {
            timerCheck = new DispatcherTimer();
            timerCheck.Interval = TimeSpan.FromMinutes(time);
            timerCheck.Tick += new EventHandler(timerCheck_Elapsed);
            timerCheck.Start();
        }

        private void timerCheck_Elapsed(object sender, EventArgs e)
        {
            timerCheck.Stop();
            var totalDrank = 0;
            foreach (var item in ManipulateList.DrinkingList)
            {
                totalDrank += item.QuantityDrank;
            }
            _ = new Schedule(Goal, totalDrank, ManipulateList);
        }
    }
}