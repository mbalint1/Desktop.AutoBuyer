using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBuyer.Core.Enums;

namespace AutoBuyer.Core.Interfaces
{
    public interface IPuppetMaster
    {
        bool ProcessingInterrupted { get; set; }
        InterruptScreen CurrentInterrupt { get; set; }
        void NavigateToTransferSearch();
        void SetSearchParameters();
        void BuyPlayers();
        void BuyConsumables();
    }
}