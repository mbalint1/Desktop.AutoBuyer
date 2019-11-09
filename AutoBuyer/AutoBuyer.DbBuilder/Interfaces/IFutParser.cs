using System.Collections.Generic;
using AutoBuyer.Data.DTO;

namespace AutoBuyer.Data.Interfaces
{
    public interface IFutParser
    {
        List<Player> GetAllFutPlayers();
    }
}