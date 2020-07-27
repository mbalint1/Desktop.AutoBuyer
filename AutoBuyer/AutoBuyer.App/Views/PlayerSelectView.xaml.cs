using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Threading;
using AutoBuyer.Core.API;
using AutoBuyer.Core.Controllers;
using AutoBuyer.Core.Data;
using AutoBuyer.Core.Interfaces;
using AutoBuyer.Core.Models;
using Player = AutoBuyer.Data.DTO.Player;

namespace AutoBuyer.App.Views
{
    /// <summary>
    /// Interaction logic for PlayerSelectView.xaml
    /// </summary>
    public partial class PlayerSelectView : Window
    {
        #region Unmanaged Code

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        #endregion Unmanaged Code

        #region Properties

        public Player SelectedPlayer { get; set; }

        public ObservableCollection<Player> PlayerList { get; set; }

        public UserPreferences UserPrefs { get; }

        public ScreenController screenController { get; set; }

        private ILogger Logger { get; set; }

        private bool AutoSellMode { get; set; }

        private bool AutoRecover { get; set; }

        public string AccessToken { get; }

        public List<int> AllPrices { get; }

        public ApiProvider Api { get; }

        #endregion Properties

        #region Constructors

        public PlayerSelectView(ILogger logger, string accessToken)
        {
            var dataProvider = new DataProvider();
            Logger = logger;

            InitializeComponent();
            Visibility = Visibility.Hidden;
            Loaded += PlayerSelectView_Loaded;
            Visibility = Visibility.Visible;

            Api = new ApiProvider();

            var players = Api.GetAllPlayers(accessToken);
            PlayerList = new ObservableCollection<Player>(players);
            txtPlayerToBuy.ItemsSource = PlayerList.Select(x => x.Name);

            UserPrefs = dataProvider.GetUserPrefs();
            AllPrices = new DataProvider().GetMaxPriceList(SelectedPlayer?.Name);

            SetMaxPriceList();
            SetMaxPlayersList();
            SetVisibility();

            screenController = new ScreenController();

            AccessToken = accessToken;
        }

        #endregion Constructors

        #region Events

        void PlayerSelectView_Loaded(object sender, RoutedEventArgs e)
        {
            Top = 0;
            Left = 0;
            var screenBounds = Screen.PrimaryScreen.Bounds;
            Width = screenBounds.Width / 2;
            Height = screenBounds.Height;
            lstPurchasedPlayers.Width = screenBounds.Width / 2;
            lstPurchasedPlayers.Height = screenBounds.Height / 2;

            screenController.OpenBrowser();
        }

        private void TxtPlayerToBuy_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: There must be a better way. Each keystroke we are iterating through the list..
            SelectedPlayer = PlayerList.FirstOrDefault(x => x.Name == txtPlayerToBuy.SelectedItem?.ToString());

            if (PlayerList.Any(x => x.Name == SelectedPlayer.Name))
            {
                chkValidPlayer.IsChecked = true;
                chkValidPlayer.Visibility = Visibility.Visible;
            }
            else
            {
                SetVisibility();
            }
        }

        private void CboMaxPrice_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMaxPrice.SelectedIndex != -1)
            {
                chkValidPrice.IsChecked = true;
            }
        }

        private void CboMaxPlayers_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMaxPlayers.SelectedIndex != -1)
            {
                chkValidMaxPlayers.IsChecked = true;
            }
        }

        private void ChkValidPlayer_OnChecked(object sender, RoutedEventArgs e)
        {
            lblMaxPrice.Visibility = Visibility.Visible;
            cboMaxPrice.Visibility = Visibility.Visible;
        }

        private void ChkValidPrice_OnChecked(object sender, RoutedEventArgs e)
        {
            chkValidPrice.Visibility = Visibility.Visible;
            lblMaxPlayers.Visibility = Visibility.Visible;
            cboMaxPlayers.Visibility = Visibility.Visible;
            lblMaxPlayers.Text = $"How many {SelectedPlayer.Name} cards do you want?";
        }

        private void chkValidMaxPlayers_Checked(object sender, RoutedEventArgs e)
        {
            chkValidMaxPlayers.Visibility = Visibility.Visible;
            btnStart.Visibility = Visibility.Visible;

            lblAutoResell.Visibility = Visibility.Visible;
            lblAutoRecover.Visibility = Visibility.Visible;
            chkAutoSell.Visibility = Visibility.Visible;
            chkAutoRecover.Visibility = Visibility.Visible;
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentSession.Reset();

            //TODO: This will need modified once we add multiple versions of a player
            var playerVersionId = SelectedPlayer.Versions.First().VersionId;
            var numberToBuy = Convert.ToInt32(cboMaxPlayers.SelectedItem);

            CurrentSession.Current = new SessionDTO
            {
                AccessToken = AccessToken,
                PlayerVersionId = playerVersionId,
                SearchNum = numberToBuy,
                PurchasedNum = 0,
                Captcha = false
            };

            var canGetPlayer = Api.TryLockPlayerForSearch();

            if (canGetPlayer)
            {
                var price = cboMaxPrice.SelectedItem.ToString();

                int.TryParse(cboMinSell.SelectedValue?.ToString(), out var minPrice);
                int.TryParse(cboMaxSell.SelectedValue?.ToString(), out var maxPrice);

                var playerObject = new Core.Models.Player
                {
                    Name = SelectedPlayer.Name,
                    NumberToPurchase = numberToBuy,
                    MaxPurchasePrice = price,
                    AutoSell = AutoSellMode,
                    SellMin = minPrice,
                    SellMax = maxPrice,
                    PlayerVersionId = playerVersionId
                };

                Logger.Log(LogType.Info, $"Program started. Searching for {numberToBuy} cards for Player: {SelectedPlayer.Name} at {price} price");

                IPuppetMaster puppetMaster = new PuppetMaster(screenController, playerObject, Logger, AutoRecover);
                puppetMaster.NavigateToTransferSearch();
                puppetMaster.SetSearchParameters();

                //TODO: Remove Debug Code
                Thread.Sleep(7000);

                Task.Factory.StartNew(() => PuppetMaster_Go(puppetMaster));
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Player is currently in use by another user");
            }
        }

        private void PuppetMaster_Go(IPuppetMaster master)
        {
            try
            {
                master.BuyPlayers();
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, $"Error in program: {ex.Message} \n{ex.StackTrace}");
                System.Windows.Forms.MessageBox.Show("Error in program. Please close program and browser and try again");
            }
        }

        private void ChkAutoSell_OnChecked(object sender, RoutedEventArgs e)
        {
            var purchasePrice = Convert.ToInt32(cboMaxPrice.Text);
            var minProfitable = (purchasePrice * .05) + purchasePrice;
            var roundedUp = (int)Math.Ceiling(minProfitable);
            var minSell = AllPrices.First(price => price > roundedUp);
            var maxSell = AllPrices.First(sellPrice => sellPrice > minSell);

            cboMinSell.ItemsSource = AllPrices.Where(x => x >= minSell).Take(100);
            cboMaxSell.ItemsSource = AllPrices.Where(x => x >= maxSell).Take(100);

            cboMinSell.SelectedItem = minSell;
            cboMaxSell.SelectedItem = maxSell;

            AutoSellMode = true;
            cboMinSell.Visibility = Visibility.Visible;
            cboMaxSell.Visibility = Visibility.Visible;
            lblMinSell.Visibility = Visibility.Visible;
            lblMaxSell.Visibility = Visibility.Visible;
        }

        private void ChkAutoSell_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AutoSellMode = false;
            cboMinSell.Visibility = Visibility.Hidden;
            cboMaxSell.Visibility = Visibility.Hidden;
            lblMinSell.Visibility = Visibility.Hidden;
            lblMaxSell.Visibility = Visibility.Hidden;
        }

        private bool Filter(string search, object item)
        {
            var playerCandidate = item.ToString().ToLower().Replace(" ", string.Empty);

            return playerCandidate.Contains(search.ToLower());
        }

        private void CboMinSell_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = (System.Windows.Controls.ComboBox)sender;

            var goodInt = int.TryParse(combo?.SelectedItem?.ToString(), out var selectedMin);

            if (goodInt)
            {
                var newSelectedMax = AllPrices.First(x => x > selectedMin);
                cboMaxSell.SelectedItem = newSelectedMax;
            }
        }

        private void CboMaxSell_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ChkAutoRecover_OnChecked(object sender, RoutedEventArgs e)
        {
            AutoRecover = true;
        }

        private void ChkAutoRecover_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AutoRecover = false;
        }

        #endregion Events

        #region Private Methods

        private void SetMaxPriceList()
        {
            cboMaxPrice.ItemsSource = AllPrices;
        }

        private void SetMaxPlayersList()
        {
            cboMaxPlayers.ItemsSource = new DataProvider().GetMaxPlayersList(SelectedPlayer?.Name);
        }

        private void SetVisibility()
        {
            lblMaxPrice.Visibility = Visibility.Hidden;
            cboMaxPrice.Visibility = Visibility.Hidden;
            lblMaxPlayers.Visibility = Visibility.Hidden;
            cboMaxPlayers.Visibility = Visibility.Hidden;
            btnStart.Visibility = Visibility.Hidden;
            chkValidPlayer.Visibility = Visibility.Hidden;
            chkValidPrice.Visibility = Visibility.Hidden;
            chkValidMaxPlayers.Visibility = Visibility.Hidden;
            lstPurchasedPlayers.Visibility = Visibility.Hidden;

            chkValidPlayer.IsChecked = false;
            chkValidMaxPlayers.IsChecked = false;
            chkValidPrice.IsChecked = false;
            chkAutoSell.IsChecked = false;
            chkAutoRecover.IsChecked = false;

            cboMaxPlayers.SelectedIndex = -1;
            cboMaxPrice.SelectedIndex = -1;

            lblAutoResell.Visibility = Visibility.Hidden;
            lblAutoRecover.Visibility = Visibility.Hidden;
            lblMinSell.Visibility = Visibility.Hidden;
            lblMaxSell.Visibility = Visibility.Hidden;
            cboMinSell.Visibility = Visibility.Hidden;
            cboMaxSell.Visibility = Visibility.Hidden;
            chkAutoSell.Visibility = Visibility.Hidden;
            chkAutoRecover.Visibility = Visibility.Hidden;
        }

        #endregion Private Methods
    }
}
