using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuyer.Core.Data
{
    public interface ILogger
    {
        void Log(LogType logType, string message);
    }
}