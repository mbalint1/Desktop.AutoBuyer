using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using AutoBuyer.Core.API;
using AutoBuyer.Core.Data;
using FontAwesome.WPF;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AutoBuyer.App.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private ILogger Logger => new Logger();

        private bool _messageBoxActive;

        public Login()
        {
            InitializeComponent();

            //WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var screenBounds = Screen.PrimaryScreen.Bounds;
            Top = screenBounds.Height / 3;
            Left = 0;
            Width = screenBounds.Width / 3;
            Height = screenBounds.Height / 3;

            txtUserName.Focus();

            spnLogin.Visibility = Visibility.Hidden;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                btnLogin.IsEnabled = false;
                spnLogin.Visibility = Visibility.Visible;
            });

            var user = txtUserName.Text.Trim();
            var password = txtPassword.Password.Trim();

            var token = await Task.Run(() => GetToken(user, password));

            if (token?.Length > 0)
            {
                if (rdoPlayer.IsChecked != null && (bool)rdoPlayer.IsChecked)
                {
                    var playerView = new PlayerSelectView(Logger, token);
                    Close();
                    playerView.Show();
                }
                else
                {
                    var consumableView = new ConsumableSelectView(Logger, token);
                    Close();
                    consumableView.Show();
                }
            }
            else
            {
                _messageBoxActive = true;
                MessageBox.Show("Invalid User or Password", "Authentication Error");
                btnLogin.IsEnabled = true;
            }

            spnLogin.Visibility = Visibility.Hidden;
        }

        private string GetToken(string user, string password)
        {
            return new ApiProvider().TokenAuthenticate(user, password);
        }

        private void TxtPassword_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!_messageBoxActive)
            {
                if (e.Key == Key.Enter)
                {
                    btnLogin_Click(sender, e);
                }
            }
            else
            {
                _messageBoxActive = false;
            }
        }
    }
}