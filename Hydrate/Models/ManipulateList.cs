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
    internal class ManipulateList : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ManipulateList()
        {
            DrinkingList = new ObservableCollection<DrinkingListItem> { };
            var dbSync = new DatabaseSync();
            Task.Run(() => dbSync.Refresh()).Wait();

            foreach (var item in dbSync.SyncList)
            {
                DrinkingList.Add(new DrinkingListItem()
                {
                    Id = item.Id,
                    QuantityDrank = int.Parse(item.DrankQuantity),
                    DrankTime = DateTime.ParseExact(item.DrankTime, "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture)
                });
            }
        }

        public void AddItem()
        {
            var timeNow = DateTime.Now;
            new DatabaseSync().Upload(timeNow, 100);

            var tempList = new List<DrinkingListItem>(DrinkingList);
            tempList.Add(new DrinkingListItem() { QuantityDrank = 100, DrankTime = timeNow, Id = timeNow.ToString("HHmmssff") });
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }

        public void DeleteItem(DrinkingListItem deleteItem)
        {
            new DatabaseSync().Delete(deleteItem.Id);

            var tempList = new List<DrinkingListItem>(DrinkingList);
            tempList.Remove(deleteItem);
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }
    }
}