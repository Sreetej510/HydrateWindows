using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrate.Models
{
    public class ManipulateList : INotifyPropertyChanged
    {
        #region Singleton

        private static ManipulateList _instance = new ManipulateList();

        public static ManipulateList GetManipulateList()
        {
            return _instance;
        }

        #endregion Singleton

        private ObservableCollection<DrinkingListItem> _drinkingList;

        private string Today;
        private string Yesterday;

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
        private int _goal;

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
        public int YesterdayValue { get; private set; }

        public int Goal
        {
            get { return _goal;  }
            set { 
                _goal = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ManipulateList()
        {
            Database.DatabaseSet("hydrate", "hydrateData");

            Today = DateTime.Now.ToString("dd-MM-yyyy");
            Yesterday = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");

            deleteOldRecord();
            DrinkingList = new ObservableCollection<DrinkingListItem> { };
            ListRefresh();
        }

        public void ListRefresh()
        {

            if(Today != DateTime.Now.ToString("dd-MM-yyyy")) {
                _instance = new ManipulateList();
                return;
            }

            new Database().post("_id", Today, "", "find", true, "").onSuccessSync(data => {
                Goal = (int)(long)data["goal"];
                object log = data["log"];
                tempListDic = JObject.FromObject(log).ToObject<Dictionary<string, object>>();
                if (tempListDic != null)
                {

                    var keys = tempListDic.Keys;
                    TotalDrank = 0;
                    if (keys != null)
                    {
                        var tempList = new List<DrinkingListItem> { };

                        foreach (var item in tempListDic)
                        {
                            var key = item.Key;

                            var value = item.Value;

                            var dic = JsonConvert.SerializeObject(value);

                            var itemObj = JsonConvert.DeserializeObject<DatabaseItem>(dic);

                            tempList.Add(new DrinkingListItem(itemObj.EatenFood, int.Parse(itemObj.DrankQuantity))
                            {
                                Id = itemObj.Id,
                                DrankTime = DateTime.ParseExact(Today + " " + itemObj.DrankTime, "dd-MM-yyyy HH.mm.ss", CultureInfo.InvariantCulture),
                            });
                            TotalDrank += int.Parse(itemObj.DrankQuantity);
                        }

                        DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));

                    }
                }
                else
                {
                    DrinkingList = new ObservableCollection<DrinkingListItem>();
                    TotalDrank = 0;
                }

            });


            
        }

        public void AddItem(object param)
        {
            var timeNow = DateTime.Now;
            var value = param.ToString();
            bool eaten;
            int quantityDrank;
            if (value == "food")
            {
                value = "0";
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

            var doc = Database.createFilter("log." + timeNow.ToString("HHmmssfff"), tempObj);
            new Database().post("_id", Today, "$set", "update", true, doc);

            UpdateTotalDrank();
        }

        public void DeleteItem(DrinkingListItem deleteItem)
        {
            var tempList = new List<DrinkingListItem>(DrinkingList);

            tempList.Remove(deleteItem);
            DrinkingList = new ObservableCollection<DrinkingListItem>(tempList.OrderByDescending(x => x.DrankTime));

            tempListDic.Remove(deleteItem.Id);

            var document = Database.createFilter("log." + deleteItem.Id, 1);
            new Database().post("_id", Today, "$unset", "update", true, document);
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

            var document = Database.createFilter("log." + editItem.Id, tempObj);
            new Database().post("_id", Today, "$set", "update", true, document);
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
            var document = Database.createFilter(Today, TotalDrank);
            new Database().post("_id", "dailyProgress", "$set", "update", true, document);
        }

        internal void getOldRecord()
        {
            new Database().post("_id", "dailyProgress", "", "find", true, "").onSuccessSync(data =>
            {
            if (data != null)
                {
                YesterdayValue = (int)(long)(data[Yesterday]);
                }
            });
        }

        internal void deleteOldRecord()
        {
            try
            {
                new Database().post("_id", Yesterday, "", "find", true, "").onSuccessSync(data =>
                {
                    if (data != null)
                    {
                        new Database().post("_id", Yesterday, "", "delete", true, "");
                        var dic = new Dictionary<string, object>() {
                            {"_id", Today },
                            {"goal",4500 },
                            {"log", new Dictionary<string, object>() }
                        };
                        new Database().post("", "", "", "insert", true, dic);
                    }
                });
                
            }
            catch {}
            getOldRecord();

            var document = Database.createFilter(Today, TotalDrank);
            new Database().post("_id", "dailyProgress", "$set", "update", true, document);
        }
    }
}