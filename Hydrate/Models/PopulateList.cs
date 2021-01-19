using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrate.Models
{
    internal class PopulateList : INotifyPropertyChanged
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

        public PopulateList()
        {
            var drinkingList = new ObservableCollection<DrinkingListItem> { };
            for (int i = 0; i < 3; i++)
            {
                drinkingList.Add(new DrinkingListItem { QuantityDrank = 100, DrankTime = new DateTime(2015, 12, 2) });
                drinkingList.Add(new DrinkingListItem { QuantityDrank = 250, DrankTime = DateTime.Now });
                drinkingList.Add(new DrinkingListItem { QuantityDrank = 150, DrankTime = new DateTime(2015, 12, 2) });
            }

            DrinkingList = new ObservableCollection<DrinkingListItem>(drinkingList.OrderByDescending(x => x.DrankTime));
        }

        public void AddItem()
        {
            var tempList = new List<DrinkingListItem>(DrinkingList);

            tempList.Add(new DrinkingListItem() { QuantityDrank = 100, DrankTime = DateTime.Now.AddHours(2) });
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }

        public void DeleteItem(DrinkingListItem deleteItem)
        {
            var tempList = new List<DrinkingListItem>(DrinkingList);

            tempList.Remove(deleteItem);
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }

        public void EditItem(DrinkingListItem editItem)
        {
            var tempList = new List<DrinkingListItem>(DrinkingList);

            tempList.Add(new DrinkingListItem() { QuantityDrank = 10000, DrankTime = DateTime.Now.AddHours(2) });
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }
    }
}