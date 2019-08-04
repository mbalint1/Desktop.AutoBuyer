using AutoBuyer.Core.Enums;

namespace AutoBuyer.Core.Interfaces
{
    public interface ISearchObject
    {
        int NumberToPurchase { get; set; }

        string MaxPurchasePrice { get; set; }

        RunMode Mode { get; }
    }
}