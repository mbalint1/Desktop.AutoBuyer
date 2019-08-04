using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuyer.Core.Data
{
    public class Logger : ILogger
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Logger()
        {
            SetupLoggingConfiguration();
        }

        public void Log(LogType logType, string message)
        {
            switch (logType)
            {
                case LogType.Info:
                    log.Info(message);
                    break;
                case LogType.Warning:
                    log.Warn(message);
                    break;
                case LogType.Error:
                    log.Error(message);
                    break;
            }
        }

        private void SetupLoggingConfiguration()
        {
        }
    }
}