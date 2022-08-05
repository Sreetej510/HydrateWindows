using Hydrate.Services;
using Hydrate.ViewModels;
using Hydrate.Views.Main;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hydrate.Models
{
    public class DrinkingListItem
    {
        public string Id { get; set; }
        public bool Eaten { get; set; }
        public string QuantityVisibility { get; set; }
        public string FoodVisibility { get; set; }
        public DateTime DrankTime { get; set; }
        private readonly static string[] ImgSourceArray = new string[] { "150ml.png", "200ml.png", "250ml.png", "330ml.png", "food.png" };

        public ICommand DeleteItem { get; }
        public ICommand EditItem { get; }
        public ManipulateList PopulateList { get; }
        public string ImageSource { get; set; }
        public int QuantityDrank { get; set; }

        public DrinkingListItem(bool eaten, int quantityDrank)
        {
            DeleteItem = new RelayCommand(p => true, p => EventDeleteItem());
            EditItem = new RelayCommand(p => true, p => EditItemModal());
            PopulateList = ManipulateList.GetManipulateList();
            QuantityDrank = quantityDrank;
            Eaten = eaten;
            if (Eaten)
            {
                QuantityVisibility = "Hidden";
                FoodVisibility = "Visible";
            }
            else
            {
                QuantityVisibility = "Visible";
                FoodVisibility = "Hidden";
            }
            SetImage();
        }

        public void SetImage()
        {
            ImageSource = "/Resources/QuantityIcons/";
            if (Eaten)
            {
                ImageSource += ImgSourceArray[4];
            }
            else
            {
                if (QuantityDrank >= 300)
                {
                    ImageSource += ImgSourceArray[3];
                }
                else if (QuantityDrank >= 250 && QuantityDrank < 300)
                {
                    ImageSource += ImgSourceArray[2];
                }
                else if (QuantityDrank >= 200 && QuantityDrank < 250)
                {
                    ImageSource += ImgSourceArray[1];
                }
                else if (QuantityDrank < 200)
                {
                    ImageSource += ImgSourceArray[0];
                }
            }
        }

        public void EditItemModal()
        {
            var editPage = new EditWindow(this);
            editPage.ShowDialog();
        }

        public void EventDeleteItem()
        {
            
            ManipulateList.GetManipulateList().DeleteItem(this);
            MainWindowViewModel.GetMainWindowViewModel().UpdateTotalDrank();
        }

        public void EditInfo(int quantity, int hour, int minutes, bool eaten)
        {
            Eaten = eaten;
            if (eaten)
            {
                QuantityDrank = 0;
                QuantityVisibility = "Hidden";
                FoodVisibility = "Visible";
            }
            else
            {
                QuantityDrank = quantity;
                QuantityVisibility = "Visible";
                FoodVisibility = "Hidden";
            }
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
            Task.Run(() => SetImage());
        }
    }
}