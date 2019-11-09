using System.Collections.Generic;
using AutoBuyer.Data.DTO;

namespace AutoBuyer.Data.Interfaces
{
    public interface IDbProvider
    {
        void InsertPlayers(List<Player> players);

        void InsertTransactionLogs(List<TransactionLog> logs);
    }
}