using System;
using System.Windows;
using AutoBuyer.Core.API;
using AutoBuyer.Core.Data;
using AutoBuyer.Core.Models;

namespace AutoBuyer.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            var sessionInfo = new SessionDTO
            {
                SessionId = CurrentSession.SessionId,
                PlayerVersionId = CurrentSession.PlayerVersionId
            };

            new ApiProvider().UpdateSession(sessionInfo, CurrentSession.Token);
        }
    }
}
