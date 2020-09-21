using System.Windows;
using AutoBuyer.Core.API;

namespace AutoBuyer.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            new ApiProvider().EndSession();
        }
    }
}
