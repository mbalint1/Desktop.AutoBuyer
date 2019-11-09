using System;
using System.Collections.Generic;
using AutoBuyer.Data.DTO;
using AutoBuyer.Data.Enums;
using AutoBuyer.Data.Interfaces;
using AutoBuyer.Data.Utilities;

namespace AutoBuyer.Data
{
    public class FutProvider
    {
        private readonly IFutParser _futParser;

        public FutProvider(FutSource futSource)
        {
            //TODO: Factory instead? Seems like overkill at the moment
            switch (futSource)
            {
                case FutSource.Futbin:
                    _futParser = new FutbinParser(new WebUtilities());
                    break;
                case FutSource.Futwiz:
                    _futParser = new FutwizParser(new WebUtilities());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(futSource), futSource, null);
            }
        }

        public IList<Player> GetFutPlayers()
        {
            return _futParser.GetAllFutPlayers();
        }

        //TODO expand to get players by type

        //TODO expand to provider real time price watching
    }
}