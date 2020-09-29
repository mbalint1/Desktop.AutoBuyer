using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using AutoBuyer.Core.API;
using AutoBuyer.Core.Data;
using FontAwesome.WPF;
using Newtonsoft.Json;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AutoBuyer.App.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
#pragma warning disable CS1106 // Extension method must be defined in a non-generic static class
    public partial class Login : Window
#pragma warning restore CS1106 // Extension method must be defined in a non-generic static class
    {
        private ILogger Logger => new Logger();

        private bool _messageBoxActive;

        private bool _rememberCreds;

        private string _prefsPath;

        public Login()
        {
            InitializeComponent();

            //WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var screenBounds = Screen.PrimaryScreen.Bounds;
            Top = screenBounds.Height / 3;
            Left = 0;
            Width = screenBounds.Width / 3;
            Height = screenBounds.Height / 3;

            spnLogin.Visibility = Visibility.Hidden;
            _rememberCreds = false;

            _prefsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoBuyer\\userPrefs.json");

            var savedCreds = CheckForStoredCreds();

            if (savedCreds)
            {
                btnLogin.Focus();
            }
            else
            {
                txtUserName.Focus();
            }
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
                if (_rememberCreds)
                {
                    CreateOrUpdateStoredCreds(user, password);
                }

                if (rdoPlayer.IsChecked != null && (bool)rdoPlayer.IsChecked)
                {
                    var playerView = new PlayerSelectView(Logger, token);
                    Close();
                    playerView.Show();
                }
                else
                {
                    MessageBox.Show("This mode is currently under construction!", "Sorry");
                    btnLogin.IsEnabled = true;

                    //var consumableView = new ConsumableSelectView(Logger, token);
                    //Close();
                    //consumableView.Show();
                }
            }
            else
            {
                _messageBoxActive = true;
                MessageBox.Show("Invalid User or Password", "Authentication Error");
                btnLogin.IsEnabled = true;
                chkRemember.IsChecked = false;
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

        private void ChkRemember_OnChecked(object sender, RoutedEventArgs e)
        {
            _rememberCreds = true;
        }

        private void ChkRemember_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _rememberCreds = true;
        }

        private void CreateOrUpdateStoredCreds(string user, string password)
        {
            try
            {
                var creds = new StoredCreds(Encrypt(user), Encrypt(password));

                using (var file = File.CreateText(_prefsPath))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, creds);
                }
            }
            catch (Exception ex)
            {
                //TODO: Log
            }
        }

        private bool CheckForStoredCreds()
        {
            try
            {
                if (File.Exists(_prefsPath))
                {
                    using (var file = File.OpenText(_prefsPath))
                    {
                        var serializer = new JsonSerializer();
                        var deserialized = (StoredCreds)serializer.Deserialize(file, typeof(StoredCreds));

                        txtUserName.Text = Decrypt(deserialized.User);
                        txtPassword.Password = Decrypt(deserialized.Password);
                    }

                    chkRemember.IsChecked = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                //TODO: Log
            }

            return false;
        }

        public static string Encrypt(string text)
        {
            return Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(text), null, DataProtectionScope.CurrentUser));
        }

        public static string Decrypt(string text)
        {
            return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(text), null, DataProtectionScope.CurrentUser));
        }

        private class StoredCreds
        {
            // Do not remove setters. They are necessary because of a bug with JSON serialize/deserialize
            public string User { get; set; }

            public string Password { get; set; }

            public StoredCreds(string user, string pass)
            {
                User = user;
                Password = pass;
            }
        }
    }
}