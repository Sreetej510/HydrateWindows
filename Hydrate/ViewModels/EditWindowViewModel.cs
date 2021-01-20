using Hydrate.Models;
using Hydrate.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        public ICommand DoneClicked { get; }

        public EditWindowViewModel(DrinkingListItem editItem, Window window)
        {
            Quantity = editItem.QuantityDrank;
            EditItem = editItem;
            Hour = EditItem.DrankTime.Hour;
            Minutes = EditItem.DrankTime.Minute;
            DoneClicked = new RelayCommand(p => true, p => EventDoneClicked(window));
        }

        private void EventDoneClicked(Window window)
        {
            EditItem.EditInfo(Quantity, Hour, Minutes);
            window.Close();
        }
    }
}