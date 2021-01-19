using Hydrate.Models;
using Hydrate.Services;
using Hydrate.Views.Main;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Hydrate.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private PopulateList _populateList;

        public PopulateList PopulateList
        {
            get { return _populateList; }
            set
            {
                _populateList = value;
                OnPropertyChanged();
            }
        }

        private DrinkingListItem _selectedItem;

        public DrinkingListItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public double EndAngle
        {
            get
            {
                double endAngle = -120.00 + ((double)TotalDrank / (Goal * 1000)) * 240;

                if (endAngle >= 120)
                {
                    endAngle = 120;
                }
                return endAngle;
            }
        }

        public double Goal { get; set; }

        private int _totalDrank;

        public int TotalDrank
        {
            get { return _totalDrank; }
            set
            {
                _totalDrank = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EndAngle));
            }
        }

        public ICommand AddItem { get; }
        public ICommand EditItem { get; }
        public ICommand DeleteItem { get; }

        // INotify
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //ctor
        public MainWindowViewModel()
        {
            Goal = 3.65;
            PopulateList = new PopulateList();
            AddItem = new RelayCommand(p => true, p => EventAddItem());
            DeleteItem = new RelayCommand(p => true, p => EventDeleteItem());
            EditItem = new RelayCommand(p => true, p => EditItemModal());
            UpdateTotalDrank();
        }

        public void EditItemModal()
        {
            var editPage = new EditWindow(SelectedItem);
            editPage.ShowDialog();
            UpdateTotalDrank();
        }

        public void EventDeleteItem()
        {
            PopulateList.DeleteItem(SelectedItem);
            UpdateTotalDrank();
        }

        public void EventAddItem()
        {
            PopulateList.AddItem();
            UpdateTotalDrank();
        }

        private void UpdateTotalDrank()
        {
            TotalDrank = 0;
            foreach (var item in PopulateList.DrinkingList)
            {
                TotalDrank += item.QuantityDrank;
            }
        }
    }
}