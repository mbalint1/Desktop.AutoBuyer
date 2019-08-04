using AutoBuyer.Core.Enums;
using AutoBuyer.Core.Interfaces;

namespace AutoBuyer.Core.Models
{
    public class Consumable : ISearchObject
    {
        public int NumberToPurchase { get; set; }
        public string MaxPurchasePrice { get; set; }
        public RunMode Mode => RunMode.Consumable;

        public bool AutoSell { get; set; }

        public int SellMin { get; set; }

        public int SellMax { get; set; }
    }
}