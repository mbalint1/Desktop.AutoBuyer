using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuyer.Core.Interfaces
{
    public interface IPuppetMaster
    {
        bool CaptchaTime { get; set; }
        void NavigateToTransferSearch();
        void SetSearchParameters();
        void BuyPlayers();
        void BuyConsumables();
    }
}