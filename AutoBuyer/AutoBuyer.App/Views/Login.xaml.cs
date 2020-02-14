using System.Windows;
using System.Windows.Forms;
using AutoBuyer.Core.Data;

namespace AutoBuyer.App.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private ILogger Logger => new Logger();

        public Login()
        {
            InitializeComponent();

            //WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var screenBounds = Screen.PrimaryScreen.Bounds;
            Top = screenBounds.Height / 3;
            Left = 0;
            Width = screenBounds.Width / 3;
            Height = screenBounds.Height / 3;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (rdoPlayer.IsChecked != null && (bool)rdoPlayer.IsChecked)
            {
                var playerView = new PlayerSelectView(Logger);
                playerView.Show();
                Close();
            }
            else
            {
                var consumableView = new ConsumableSelectView(Logger);
                consumableView.Show();
                Close();
            }
        }
    }
}