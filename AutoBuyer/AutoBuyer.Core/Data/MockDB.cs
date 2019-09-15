using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoBuyer.Core.Models;

namespace AutoBuyer.Core.Data
{
    internal class MockDB
    {
        public List<string> ReadPlayerList()
        {
            try
            {
                var playerNames = new List<string>();

                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoBuyer\\PlayerDB.csv");

                var csvLines = File.ReadAllLines(filePath);

                foreach (var line in csvLines)
                {
                    var name = line.Split(',').FirstOrDefault();
                    playerNames.Add(name);
                }

                return playerNames;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void LoadButtonCoordinates()
        {

        }

        public UserPreferences GetUserPrefs()
        {
            // Read from DB?

            return new UserPreferences
            {
                FirstTimeUser = false,
                ValidButtonCoordinates = false
            };
        }
    }
}