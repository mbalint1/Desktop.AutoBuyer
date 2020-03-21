using System;
using System.Collections.Generic;
using System.Linq;
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
using AutoBuyer.Core.Controllers;
using AutoBuyer.Core.Data;
using AutoBuyer.Core.Interfaces;
using AutoBuyer.Core.Models;

namespace AutoBuyer.App.Views
{
    /// <summary>
    /// Interaction logic for ConsumableSelectView.xaml
    /// </summary>
    public partial class ConsumableSelectView : Window
    {
        private ILogger Logger { get; set; }

        public ScreenController screenController { get; set; }

        public bool AutoSellMode { get; private set; }

        public string AccessToken { get; }

        public ConsumableSelectView(ILogger logger, string token)
        {
            InitializeComponent();

            Logger = logger;
            screenController = new ScreenController();
            AccessToken = token;

            Loaded += OnLoaded;

            SetMaxPlayersList();
            SetVisibility();

            lblInstructions.Text = $@"**Before clicking Start button do the following**:
Navigate to Transfer Search
Click Consumables tab
Select the consumable you want to purchase
Enter the Max BIN price you wish to pay (pick an item that is >= 500 coins as the app will toggle min price from 200-250)";
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
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

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            var numberToBuy = Convert.ToInt32(cboMaxConsumables.SelectedItem);

            var minParse = int.TryParse(txtMinSell.Text, out var minPrice);
            var maxParse = int.TryParse(txtMaxSell.Text, out var maxPrice);

            //TODO: Fix this correctly
            minParse = true;
            maxParse = true;

            if (minParse && maxParse)
            {
                Logger.Log(LogType.Info, $"Program started. Searching for {numberToBuy} consumables.");

                var consumable = new Consumable
                {
                    NumberToPurchase = numberToBuy,
                    AutoSell = AutoSellMode,
                    SellMin = minPrice,
                    SellMax = maxPrice
                };

                IPuppetMaster puppetMaster = new PuppetMaster(screenController, consumable, Logger, AccessToken);
                Task.Factory.StartNew(() => PuppetMaster_Go(puppetMaster));
            }


        }

        private void PuppetMaster_Go(IPuppetMaster master)
        {
            try
            {
                master.BuyConsumables();
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, $"Error in program: {ex.Message} \n{ex.StackTrace}");
                System.Windows.Forms.MessageBox.Show("Error in program. Please close program and browser and try again");
            }
        }

        private void SetMaxPlayersList()
        {
            cboMaxConsumables.ItemsSource = new DataProvider().GetMaxPlayersList(string.Empty);
        }

        private void SetVisibility()
        {
            btnStart.Visibility = Visibility.Hidden;
            lstPurchasedPlayers.Visibility = Visibility.Hidden;
            cboMaxConsumables.SelectedIndex = -1;
            lblInstructions.Visibility = Visibility.Hidden;
            lblAutoResell.Visibility = Visibility.Hidden;
            lblMinSell.Visibility = Visibility.Hidden;
            lblMaxSell.Visibility = Visibility.Hidden;
            txtMinSell.Visibility = Visibility.Hidden;
            txtMaxSell.Visibility = Visibility.Hidden;
            chkAutoSell.Visibility = Visibility.Hidden;
        }

        private void CboMaxConsumables_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMaxConsumables.SelectedIndex > -1)
            {
                btnStart.Visibility = Visibility.Visible;
                lblInstructions.Visibility = Visibility.Visible;
                lblAutoResell.Visibility = Visibility.Visible;
                chkAutoSell.Visibility = Visibility.Visible;
            }
        }

        private void ChkAutoSell_OnChecked(object sender, RoutedEventArgs e)
        {
            AutoSellMode = true;
            txtMinSell.Visibility = Visibility.Visible;
            txtMaxSell.Visibility = Visibility.Visible;
            lblMinSell.Visibility = Visibility.Visible;
            lblMaxSell.Visibility = Visibility.Visible;
        }

        private void ChkAutoSell_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AutoSellMode = false;
            txtMinSell.Visibility = Visibility.Hidden;
            txtMaxSell.Visibility = Visibility.Hidden;
            lblMinSell.Visibility = Visibility.Hidden;
            lblMaxSell.Visibility = Visibility.Hidden;
        }
    }
}
