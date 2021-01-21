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
        private ManipulateList _populateList;

        public ManipulateList PopulateList
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
            Goal = 4;
            PopulateList = new ManipulateList();
            AddItem = new RelayCommand(p => true, p => EventAddItem(p));
            EditItem = new RelayCommand(p => true, p => EditItemModal());
            DeleteItem = new RelayCommand(p => true, p => EventDeleteItem());
            UpdateTotalDrank();

            _ = new Schedule(Goal, TotalDrank, PopulateList);
        }

        public void EventAddItem(object param)
        {
            PopulateList.AddItem(param);
            UpdateTotalDrank();
        }

        public void EditItemModal()
        {
            var editPage = new EditWindow(SelectedItem, PopulateList);
            editPage.ShowDialog();
            UpdateTotalDrank();
        }

        public void EventDeleteItem()
        {
            PopulateList.DeleteItem(SelectedItem);
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