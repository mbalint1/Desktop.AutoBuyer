using System;
using System.Collections.Generic;
using AutoBuyer.Data;
using AutoBuyer.Data.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoBuyer.Tests
{
    [TestClass]
    public class DataTests
    {
        [TestMethod]
        public void InsertTransactionLog()
        {
            var transaction = new TransactionLog
            {
                Type = TransactionType.SuccessfulSale,
                PlayerName = "Michael Bolton",
                SearchPrice = 1500,
                TransactionDate = DateTime.Now
            };

            new DataProvider().SaveTransactionLog(transaction);
        }
    }
}