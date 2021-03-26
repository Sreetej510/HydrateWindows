using Google.Cloud.Firestore;
using Newtonsoft.Json;
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
    public class ManipulateList : INotifyPropertyChanged
    {
        #region Singleton

        private static readonly ManipulateList _instance = new ManipulateList();

        public static ManipulateList GetManipulateList()
        {
            return _instance;
        }

        #endregion Singleton

        private DocumentReference docRef;
        private FirestoreDb db;

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

        private DateTime _nextDrinkTime;
        private Dictionary<string, object> tempListDic;

        public DateTime NextDrinkTime
        {
            get { return _nextDrinkTime; }
            set
            {
                _nextDrinkTime = value;
                OnPropertyChanged();
            }
        }

        public int TotalDrank { get; private set; }

        public readonly int Goal = 4;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ManipulateList()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"hydrate.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            db = FirestoreDb.Create("hydrate-a8727");

            var Today = DateTime.Now.ToString("dd-MM-yyyy");

            docRef = db.Collection("hydrate").Document(Today);
            DrinkingList = new ObservableCollection<DrinkingListItem> { };
            ListRefresh();
        }

        public void ListRefresh()
        {
            var snapshot = docRef.GetSnapshotAsync().Result;

            if (!snapshot.Exists)
            {
                var Today = DateTime.Now.ToString("dd-MM-yyyy");
                var yDay = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                var p = db.Collection("hydrate").Document(Today).SetAsync(new Dictionary<string, object> { }).Result;
                db.Collection("hydrate").Document(yDay).DeleteAsync();
                snapshot = docRef.GetSnapshotAsync().Result;
            }

            tempListDic = snapshot.ToDictionary();

            var tempList = new List<DrinkingListItem> { };

            TotalDrank = 0;

            foreach (var item in tempListDic)
            {
                var key = item.Key;

                var value = item.Value;

                var dic = JsonConvert.SerializeObject(value);

                var itemObj = JsonConvert.DeserializeObject<DatabaseItem>(dic);

                tempList.Add(new DrinkingListItem(itemObj.EatenFood, int.Parse(itemObj.DrankQuantity))
                {
                    Id = itemObj.Id,
                    DrankTime = DateTime.ParseExact(snapshot.Id + " " + itemObj.DrankTime, "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture),
                });
                TotalDrank += int.Parse(itemObj.DrankQuantity);
            }

            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));
        }

        public void AddItem(object param)
        {
            var timeNow = DateTime.Now;

            var value = param.ToString();
            bool eaten;
            int quantityDrank;
            if (value == "food")
            {
                eaten = true;
                quantityDrank = 0;
            }
            else
            {
                eaten = false;
                quantityDrank = int.Parse(value);
            }

            var tempList = new List<DrinkingListItem>(DrinkingList)
            {
                new DrinkingListItem(eaten,quantityDrank) { DrankTime = timeNow, Id = timeNow.ToString("HHmmssfff")}
            };
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));

            var tempObj = new Dictionary<string, object>
            {
                { "Id" , timeNow.ToString("HHmmssfff") },
                { "DrankQuantity" , value },
                { "DrankTime" , timeNow.ToString("HH.mm.ss") },
                { "EatenFood" , eaten }
            };

            tempListDic.Add(timeNow.ToString("HHmmssfff"), tempObj);

            docRef.UpdateAsync(timeNow.ToString("HHmmssfff"), tempObj);

            UpdateTotalDrank();
        }

        public void DeleteItem(DrinkingListItem deleteItem)
        {
            var tempList = new List<DrinkingListItem>(DrinkingList);

            tempList.Remove(deleteItem);
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));

            tempListDic.Remove(deleteItem.Id);

            docRef.SetAsync(tempListDic);
            UpdateTotalDrank();
        }

        public void EditItem(DrinkingListItem editItem)
        {
            var tempList = new List<DrinkingListItem>(DrinkingList);
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));

            var tempObj = new Dictionary<string, object>
            {
                { "Id" , editItem.Id },
                { "DrankQuantity" , editItem.QuantityDrank.ToString() },
                { "DrankTime" , editItem.DrankTime.ToString("HH.mm.ss") },
                { "EatenFood" , editItem.Eaten }
            };

            docRef.UpdateAsync(editItem.Id, tempObj);

            UpdateTotalDrank();
        }

        private void UpdateTotalDrank()
        {
            TotalDrank = 0;
            foreach (var item in DrinkingList)
            {
                TotalDrank += item.QuantityDrank;
            }
        }

        public void UploadTotalDrank()
        {
            var today = DateTime.Now.ToString("dd-MM-yyyy");
            db.Collection("hydrate").Document("dailyProgress").UpdateAsync(today, TotalDrank);
        }

        internal int getOldRecord()
        {
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
            var doc = db.Collection("hydrate").Document("dailyProgress").GetSnapshotAsync().Result;
            var res = doc.GetValue<int>(yesterday);
            return res;
        }

        internal void deleteOldRecord()
        {
            var today = DateTime.Now.ToString("dd-MM-yyyy");
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
            var doc = db.Collection("hydrate").Document("dailyProgress").GetSnapshotAsync().Result;

            var todayValue = doc.GetValue<int>(today);
            var yesterdayValue = doc.GetValue<int>(yesterday);
            var obj = new Dictionary<string, object>
            {
                {today, todayValue },
                {yesterday, yesterdayValue }
            };

            db.Collection("hydrate").Document("dailyProgress").SetAsync(obj);
        }
    }
}