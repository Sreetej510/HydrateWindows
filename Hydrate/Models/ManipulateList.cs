using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Hydrate.Models
{
    public class ManipulateList : INotifyPropertyChanged
    {
        private ObservableCollection<DrinkingListItem> _drinkingList;

        public ObservableCollection<DrinkingListItem> DrinkingList
        {
            get { return _drinkingList; }
            set
            {
                _drinkingList = value;
                OnPropertyChanged();
            }
        }

        private DateTime _nextDrinkTime;

        public DateTime NextDrinkTime
        {
            get { return _nextDrinkTime; }
            set
            {
                _nextDrinkTime = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ManipulateList()
        {
            DrinkingList = new ObservableCollection<DrinkingListItem> { };
            ListRefresh();
        }

        public void ListRefresh()
        {
            var dbSync = new DatabaseSync();
            Task.Run(() => dbSync.Refresh()).Wait();

            var tempList = new List<DrinkingListItem> { };

            foreach (var item in dbSync.SyncList)
            {
                tempList.Add(new DrinkingListItem(item.EatenFood, int.Parse(item.DrankQuantity))
                {
                    Id = item.Id,
                    DrankTime = DateTime.ParseExact(item.DrankTime, "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture),
                });
            }

            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }

        public void AddItem(object param)
        {
            var timeNow = DateTime.Now;

            var value = param.ToString();
            bool eaten;
            int quantityDrank;
            if (value == "food")
            {
                eaten = true;
                quantityDrank = 0;
            }
            else
            {
                eaten = false;
                quantityDrank = int.Parse(value);
            }

            new DatabaseSync().Upload(timeNow, quantityDrank, eaten);

            var tempList = new List<DrinkingListItem>(DrinkingList)
            {
                new DrinkingListItem(eaten,quantityDrank) { DrankTime = timeNow, Id = timeNow.ToString("HHmmssfff")}
            };
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }

        public void DeleteItem(DrinkingListItem deleteItem)
        {
            new DatabaseSync().Delete(deleteItem.Id);

            var tempList = new List<DrinkingListItem>(DrinkingList);
            tempList.Remove(deleteItem);
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }

        public void EditItem()
        {
            var tempList = new List<DrinkingListItem>(DrinkingList);
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }
    }
}