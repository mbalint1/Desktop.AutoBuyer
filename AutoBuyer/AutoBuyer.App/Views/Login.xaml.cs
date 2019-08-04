using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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