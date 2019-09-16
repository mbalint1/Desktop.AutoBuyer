using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoBuyer.Core.Models;

namespace AutoBuyer.Core.Data
{
    public class DataProvider
    {
        public ObservableCollection<string> GetPlayerNames()
        {
            var playerList = new MockDB().ReadPlayerList();

            return new ObservableCollection<string>(playerList);
        }

        public List<int> GetMaxPlayersList(string playerName)
        {
            var x = new List<int>();

            for (int i = 1; i <= 100; i++)
            {
                x.Add(i);
            }
            return x;
        }

        public List<int> GetMaxPriceList(string playerName)
        {
            var tempList = new List<int>();

            for (int i = 700; i < 1000; i += 50)
            {
                tempList.Add(i);
            }
            for (int j = 1000; j < 10000; j += 100)
            {
                tempList.Add(j);
            }
            for (int k = 10000; k < 50000; k += 250)
            {
                tempList.Add(k);
            }
            for (int l = 50000; l < 100000; l += 500)
            {
                tempList.Add(l);
            }
            for (int m = 100000; m < 1500000; m += 1000)
            {
                tempList.Add(m);
            }

            //range 700 - 1,500,000
            return tempList;
        }

        public UserPreferences GetUserPrefs()
        {
            return new MockDB().GetUserPrefs();
        }

        public void LoadButtonCoordinates()
        {
            new MockDB().LoadButtonCoordinates();
        }
    }
}