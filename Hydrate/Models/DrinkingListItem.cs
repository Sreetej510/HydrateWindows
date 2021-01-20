using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Hydrate.Models
{
    public class DrinkingListItem : INotifyPropertyChanged
    {
        public string Id { get; set; }
        private DateTime _drankTime;

        public DateTime DrankTime
        {
            get { return _drankTime; }
            set
            {
                _drankTime = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ImageSource { get; set; }
        private int _quantityDrank;

        public int QuantityDrank
        {
            get { return _quantityDrank; }
            set
            {
                _quantityDrank = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DrinkingListItem()
        {
        }

        public void EditInfo(int quantity, int hour, int minutes)
        {
            QuantityDrank = quantity;
            var day = DrankTime.Day;
            var month = DrankTime.Month;
            var year = DrankTime.Year;
            if (hour >= 23)
            {
                hour = 23;
            }
            if (minutes >= 59)
            {
                minutes = 59;
            }

            DrankTime = new DateTime(year, month, day, hour, minutes, 0);
            new DatabaseSync().Edit(DrankTime, QuantityDrank, Id);
        }
    }
}