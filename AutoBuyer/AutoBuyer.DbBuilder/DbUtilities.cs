using System;
using System.Collections.Generic;
using System.IO;
using AutoBuyer.DbBuilder.DTO;

namespace AutoBuyer.DbBuilder
{
    public class DbUtilities
    {
        public void WritePlayerFile(List<string> playerCsvs)
        {
            var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoBuyer");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Path.Combine(directoryPath, "PlayerDB.csv");

            if (!File.Exists(fileName))
            {
                using (var sw = File.CreateText(fileName))
                {
                    foreach (var player in playerCsvs)
                    {
                        sw.WriteLine(player);
                    }
                }
            }
            else
            {
                using (var sw = new StreamWriter(fileName, true))
                {
                    foreach (var player in playerCsvs)
                    {
                        sw.WriteLine(player);
                    }
                }
            }
        }

        public void SavePlayers(List<Player> players)
        {

        }
    }
}