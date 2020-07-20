using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBuyer.Core.Enums;
using AutoBuyer.Core.Interfaces;

namespace AutoBuyer.Core.Models
{
    public class Player : ISearchObject
    {
        public string Name { get; set; }

        public int NumberToPurchase { get; set; }

        public string MaxPurchasePrice { get; set; }

        public RunMode Mode => RunMode.Player;

        public bool AutoSell { get; set; }

        public int SellMin { get; set; }

        public int SellMax { get; set; }

        public string PlayerVersionId { get; set; }
    }
}