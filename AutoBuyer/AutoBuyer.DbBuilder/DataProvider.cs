using System;
using System.Collections.Generic;
using System.IO;
using AutoBuyer.Data.DTO;
using AutoBuyer.Data.Interfaces;
using AutoBuyer.Data.Postgres;

namespace AutoBuyer.Data
{
    public class DataProvider
    {
        private readonly IDbProvider _dbProvider;

        public DataProvider()
        {
            _dbProvider = new PostgresProvider();
        }

        public void SavePlayers(List<Player> players)
        {
            _dbProvider.InsertPlayers(players);
        }

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

        public void SaveTransactionLog(TransactionLog log)
        {
            _dbProvider.InsertTransactionLog(log);
        }
    }
}