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
                var path = ConfigurationManager.AppSettings["DbFilePath"];
                if (File.Exists(path))
                {
                    var csv = File.ReadAllText(path);
                    return csv.Split(',').Distinct().ToList();
                }

                return null; //TODO: What here?
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