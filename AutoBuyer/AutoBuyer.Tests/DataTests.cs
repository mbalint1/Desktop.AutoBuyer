using System;
using System.Collections.Generic;
using AutoBuyer.Core.API;
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
    }
}