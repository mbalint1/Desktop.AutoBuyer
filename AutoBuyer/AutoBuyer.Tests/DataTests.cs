using System;
using System.Collections.Generic;
using System.Linq;
using AutoBuyer.Core.API;
using AutoBuyer.Data;
using AutoBuyer.Data.DTO;
using AutoBuyer.Data.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoBuyer.Tests
{
    [TestClass]
    public class DataTests
    {
        [TestMethod]
        public void InsertTransactionLog()
        {
            const string tempToken =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoibWJhbGludCIsIm5iZiI6MTU4NDMwNjA0MSwiZXhwIjoxNTg0MzQ5MjQxLCJpYXQiOjE1ODQzMDYwNDF9.tqQClMJDjDjKtGJYhFNXGqFaSHMs34LqQDTzgXFR830";

            var transaction = new TransactionLog
            {
                Type = TransactionType.SuccessfulPurchase,
                PlayerName = "Michael Bolton",
                SearchPrice = 1500,
                TransactionDate = DateTime.Now
            };

            new ApiProvider().InsertTransactionLog(transaction, tempToken);
        }

        [TestMethod]
        public void BuildFutDatabase()
        {
            try
            {
                var futProvider = new FutProvider(FutSource.Futbin);

                var players = futProvider.GetFutPlayers().ToList();

                var dataProvider = new DataProvider();

                dataProvider.SavePlayers(players);
            }
            catch (Exception ex)
            {
            }
        }
    }
}