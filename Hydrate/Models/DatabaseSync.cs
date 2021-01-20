using Firebase.Database;
using Firebase.Database.Query;
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

            SyncList = new ObservableCollection<DatabaseItem>((await FirebaseClient.Child(Today)
                .OnceAsync<DatabaseItem>()).Select(item => new DatabaseItem()
                {
                    Id = item.Object.Id,
                    DrankTime = Today + " " + item.Object.DrankTime,
                    DrankQuantity = item.Object.DrankQuantity
                }));
        }

        public async void Upload(DateTime dateTime, int drankQuantity)
        {
            await FirebaseClient.Child(DateTime.Now.ToString("dd-MM-yyyy")).Child(dateTime.ToString("HHmmssff"))
                 .PutAsync(new DatabaseItem()
                 {
                     Id = dateTime.ToString("HHmmssff"),
                     DrankQuantity = drankQuantity.ToString(),
                     DrankTime = dateTime.ToString("HH.mm.ss"),
                 });
        }

        public async void Edit(DateTime dateTime, int drankQuantity, string id)
        {
            await FirebaseClient.Child(DateTime.Now.ToString("dd-MM-yyyy")).Child(id)
                .PutAsync(new DatabaseItem()
                {
                    Id = id,
                    DrankQuantity = drankQuantity.ToString(),
                    DrankTime = dateTime.ToString("HH.mm.ss"),
                });
        }

        public async void Delete(string id)
        {
            await FirebaseClient.Child(DateTime.Now.ToString("dd-MM-yyyy")).Child(id).DeleteAsync();
        }
    }
}