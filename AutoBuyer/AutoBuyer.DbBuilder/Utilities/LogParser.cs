using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoBuyer.Data.DTO;

namespace AutoBuyer.Data.Utilities
{
    public class LogParser
    {
        public List<TransactionLog> ParsePurchaseAttempts(string logFilePath, DateTime startDate, DateTime endDate)
        {
            var transactionLogs = new List<TransactionLog>();

            var lines = File.ReadAllLines(logFilePath).ToList();

            var goodLines = lines.Where(x => IsValidTransactionLog(x, startDate, endDate)).ToList();

            var lastPlayerSearched = string.Empty;
            var lastPlayerPrice = 0;

            foreach (var line in goodLines)
            {
                if (line.Contains("Program started. Searching for"))
                {
                    var nameStart = line.LastIndexOf(":", StringComparison.Ordinal) + 2;
                    var nameEnd = line.LastIndexOf("at", StringComparison.Ordinal) - 1;

                    lastPlayerSearched = line.Substring(nameStart, nameEnd - nameStart);

                    var priceStart = line.LastIndexOf("at", StringComparison.Ordinal) + 3;
                    var priceEnd = line.LastIndexOf("price", StringComparison.Ordinal) - 1;

                    lastPlayerPrice = Convert.ToInt32(line.Substring(priceStart, priceEnd - priceStart));
                }
                else
                {
                    var dateString = line.Substring(0, 19);
                    var logDate = DateTime.Parse(dateString);

                    transactionLogs.Add(new TransactionLog
                    {
                        PlayerName = lastPlayerSearched,
                        SearchPrice = lastPlayerPrice,
                        TransactionDate = logDate,
                        Type = line.Contains("purchased successfully") ? TransactionType.SuccessfulPurchase : TransactionType.FailedPurchase
                    });
                }
            }

            return transactionLogs;
        }

        private bool IsValidTransactionLog(string line, DateTime startDate, DateTime endDate)
        {
            bool loggable = false;

            if (line.StartsWith("2019-") && line.Contains("INFO"))
            {
                if (line.Contains("purchased successfully") || line.Contains("failed to purchase") || line.Contains("Program started. Searching for"))
                {
                    var dateString = line.Substring(0, 19);
                    var logDate = DateTime.Parse(dateString);

                    if (logDate >= startDate && logDate <= endDate)
                    {
                        if (!line.ToLower().Contains("consumable"))
                        {
                            loggable = true;
                        }
                    }
                }
            }

            return loggable;
        }
    }
}