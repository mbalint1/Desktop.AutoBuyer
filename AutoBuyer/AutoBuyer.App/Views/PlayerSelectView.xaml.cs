﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Binding = System.Windows.Data.Binding;
using System.Diagnostics;
using System.Threading;
using AutoBuyer.Core;
using AutoBuyer.Core.Controllers;
using AutoBuyer.Core.Data;
using AutoBuyer.Core.Models;
using AutoBuyer.Core.Interfaces;

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

        public string SelectedPlayer { get; set; }

        public ObservableCollection<string> AvailablePlayers { get; set; }

        public UserPreferences UserPrefs { get; }

        public ScreenController screenController { get; set; }

        private ILogger Logger { get; set; }

        private bool AutoSellMode { get; set; }

        private bool AutoRecover { get; set; }

        public string AccessToken { get; }

        public List<int> AllPrices { get; }

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

            AvailablePlayers = dataProvider.GetPlayerNames();
            txtPlayerToBuy.ItemsSource = AvailablePlayers;
            UserPrefs = dataProvider.GetUserPrefs();
            AllPrices = new DataProvider().GetMaxPriceList(SelectedPlayer);

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
            SelectedPlayer = txtPlayerToBuy.SelectedItem?.ToString();

            if (AvailablePlayers.Contains(SelectedPlayer))
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
            lblMaxPlayers.Text = $"How many {SelectedPlayer} cards do you want?";
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
            var numberToBuy = Convert.ToInt32(cboMaxPlayers.SelectedItem);
            var price = cboMaxPrice.SelectedItem.ToString();

            var minParse = int.TryParse(cboMinSell.SelectedValue?.ToString(), out var minPrice);
            var maxParse = int.TryParse(cboMaxSell.SelectedValue?.ToString(), out var maxPrice);

            var playerObject = new Player
            {
                Name = SelectedPlayer,
                NumberToPurchase = numberToBuy,
                MaxPurchasePrice = price,
                AutoSell = AutoSellMode,
                SellMin = minPrice,
                SellMax = maxPrice
            };

            Logger.Log(LogType.Info, $"Program started. Searching for {numberToBuy} cards for Player: {SelectedPlayer} at {price} price");

            IPuppetMaster puppetMaster = new PuppetMaster(screenController, playerObject, Logger, AccessToken, AutoRecover);
            puppetMaster.NavigateToTransferSearch();
            puppetMaster.SetSearchParameters();

            //TODO: Remove Debug Code
            Thread.Sleep(7000);

            Task.Factory.StartNew(() => PuppetMaster_Go(puppetMaster));
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

        #endregion Events

        #region Private Methods

        private void SetMaxPriceList()
        {
            cboMaxPrice.ItemsSource = AllPrices;
        }

        private void SetMaxPlayersList()
        {
            cboMaxPlayers.ItemsSource = new DataProvider().GetMaxPlayersList(SelectedPlayer);
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
            var combo = (System.Windows.Controls.ComboBox) sender;

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
    }
}
