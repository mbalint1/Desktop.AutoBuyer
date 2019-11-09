using System;
using System.Collections.Generic;
using AutoBuyer.Data.DTO;
using AutoBuyer.Data.Utilities;

namespace AutoBuyer.Data
{
    public class LogProvider
    {
        //TODO this class is meant to be used as a logic controller for the log parser

        public List<TransactionLog> ParsePurchaseAttempts(string logFilePath, DateTime startDate, DateTime endDate)
        {
            return new LogParser().ParsePurchaseAttempts(logFilePath, startDate, endDate);
        }
    }
}