using Hydrate.Models;
using Hydrate.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hydrate.ViewModels
{
    internal class EditWindowViewModel
    {
        public DrinkingListItem EditItem { get; set; }
        public int Quantity { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }
        public bool HasEaten { get; set; }

        public ICommand DoneClicked { get; }

        public EditWindowViewModel(DrinkingListItem editItem, Window window, ManipulateList List)
        {
            Quantity = editItem.QuantityDrank;
            EditItem = editItem;
            Hour = EditItem.DrankTime.Hour;
            Minutes = EditItem.DrankTime.Minute;
            DoneClicked = new RelayCommand(p => true, p => EventDoneClicked(window, List));
            HasEaten = EditItem.Eaten;
        }

        private void EventDoneClicked(Window window, ManipulateList List)
        {
            EditItem.EditInfo(Quantity, Hour, Minutes, HasEaten);
            Task.Run(() => Schedule.GetSchedule().StartSchedule());
            List.EditItem();
            window.Close();
        }
    }
}