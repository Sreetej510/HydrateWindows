using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Hydrate.Models
{
    public class DrinkingListItem
    {
        public string Id { get; set; }
        public bool Eaten { get; set; }
        public string QuantityVisibility { get; set; }
        public string FoodVisibility { get; set; }
        public DateTime DrankTime { get; set; }
        private readonly static string[] ImgSourceArray = new string[] { "below_150ml.png", "150ml.png", "200ml.png", "250ml.png", "food.png" };

        public string ImageSource { get; set; }
        public int QuantityDrank { get; set; }

        public DrinkingListItem(bool eaten, int quantityDrank)
        {
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
                if (QuantityDrank >= 250)
                {
                    ImageSource += ImgSourceArray[3];
                }
                else if (QuantityDrank >= 200 && QuantityDrank < 250)
                {
                    ImageSource += ImgSourceArray[2];
                }
                else if (QuantityDrank >= 150 && QuantityDrank < 200)
                {
                    ImageSource += ImgSourceArray[1];
                }
                else if (QuantityDrank < 150)
                {
                    ImageSource += ImgSourceArray[0];
                }
            }
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
            new DatabaseSync().Edit(DrankTime, QuantityDrank, Id, Eaten);
            SetImage();
        }
    }
}