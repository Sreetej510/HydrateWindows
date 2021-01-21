using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrate.Models
{
    public class DatabaseSync
    {
        private readonly FirebaseClient FirebaseClient;

        public ObservableCollection<DatabaseItem> SyncList;

        //ctor
        public DatabaseSync()
        {
            FirebaseClient = new FirebaseClient("https://hydrate-a8727-default-rtdb.firebaseio.com/");
            SyncList = new ObservableCollection<DatabaseItem> { };
        }

        public async Task Refresh()
        {
            var Today = DateTime.Now.ToString("dd-MM-yyyy");

            var tempList = (await FirebaseClient.Child(Today)
                .OnceAsync<DatabaseItem>()).Select(item => new DatabaseItem()
                {
                    Id = item.Object.Id,
                    DrankTime = Today + " " + item.Object.DrankTime,
                    DrankQuantity = item.Object.DrankQuantity,
                    EatenFood = item.Object.EatenFood
                });
            SyncList = new ObservableCollection<DatabaseItem>(tempList.OrderByDescending(x => x.DrankTime));
        }

        public async void Upload(DateTime dateTime, int drankQuantity, bool eatenFood)
        {
            await FirebaseClient.Child(DateTime.Now.ToString("dd-MM-yyyy")).Child(dateTime.ToString("HHmmssff"))
                 .PutAsync(new DatabaseItem()
                 {
                     Id = dateTime.ToString("HHmmssff"),
                     DrankQuantity = drankQuantity.ToString(),
                     DrankTime = dateTime.ToString("HH.mm.ss"),
                     EatenFood = eatenFood
                 });
        }

        public async void Edit(DateTime dateTime, int drankQuantity, string id, bool eaten)
        {
            await FirebaseClient.Child(DateTime.Now.ToString("dd-MM-yyyy")).Child(id)
                .PutAsync(new DatabaseItem()
                {
                    Id = id,
                    DrankQuantity = drankQuantity.ToString(),
                    DrankTime = dateTime.ToString("HH.mm.ss"),
                    EatenFood = eaten
                });
        }

        public async void Delete(string id)
        {
            await FirebaseClient.Child(DateTime.Now.ToString("dd-MM-yyyy")).Child(id).DeleteAsync();
        }
    }
}