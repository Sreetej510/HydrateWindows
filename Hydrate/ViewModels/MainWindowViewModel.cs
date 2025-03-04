﻿using Hydrate.Models;
using Hydrate.Services;
using Hydrate.Views.Main;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hydrate.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Singleton

        private static readonly MainWindowViewModel _instance = new MainWindowViewModel();

        public static MainWindowViewModel GetMainWindowViewModel()
        {
            return _instance;
        }

        #endregion Singleton

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
                double endAngle = -120.00 + ((double)TotalDrank /PopulateList.Goal) * 240;

                if (endAngle >= 120)
                {
                    endAngle = 120;
                }
                return endAngle;
            }
        }

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

        public int YesterdayDrank { get; private set; }

        public ICommand AddItem { get; }
        public ICommand EditItem { get; }
        public ICommand DeleteItem { get; }
        public Schedule ScheduleClass { get; }

        // INotify
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //ctor
        private MainWindowViewModel()
        {
            PopulateList = ManipulateList.GetManipulateList();
            AddItem = new RelayCommand(p => true, p => EventAddItem(p));
            EditItem = new RelayCommand(p => true, p => EditItemModal());
            DeleteItem = new RelayCommand(p => true, p => EventDeleteItem());
            ScheduleClass = Schedule.GetSchedule();
            UpdateTotalDrank(true);
            YesterdayDrank = PopulateList.YesterdayValue;
            Task.Run(() => PopulateList.deleteOldRecord());
        }

        public void EventAddItem(object param)
        {
            PopulateList.AddItem(param);
            UpdateTotalDrank();
        }

        public void EditItemModal()
        {
            var editPage = new EditWindow(SelectedItem);
            editPage.ShowDialog();
        }

        public void EventDeleteItem()
        {
            PopulateList.DeleteItem(SelectedItem);
            UpdateTotalDrank();
        }

        public void UpdateTotalDrank(bool tempBool = false)
        {
            TotalDrank = PopulateList.TotalDrank;
            PopulateList.UploadTotalDrank();
            Task.Run(() => ScheduleClass.StartSchedule(tempBool));
        }
    }
}