using System;
using System.Configuration;
using System.Drawing;
using System.Runtime.InteropServices;
using AutoBuyer.Core.Enums;
using AutoBuyer.Core.Interfaces;

namespace AutoBuyer.Core.Controllers
{
    public class MouseController : IMouseController
    {
        #region Properties

        public RunMode Mode { get; }

        private Point LoginCoordinates { get; set; }

        private Point TransferNavigateCoordinates { get; set; }

        private Point SearchTransferMarketCoordinates { get; set; }

        private Point PlayerNameTextboxCoordinates { get; set; }

        private Point PlayerNameTextDropCoordinates { get; set; }

        private Point MaxPriceTextCoordinates { get; set; }

        private Point SearchCoordinates { get; set; }

        private Point BuyCoordinates { get; set; }

        private Point ConfirmCoordinates { get; set; }

        private Point SendToTransferListCoordinates { get; set; }

        private Point BackCoordinates { get; set; }

        private Point IncreaseMinConsumables { get; set; }

        private Point DecreaseMinConsumables { get; set; }

        private Point ListItemBegin { get; set; }

        private Point ListMinText { get; set; }

        private Point ListMaxText { get; set; }

        private Point ListItemFinal { get; set; }

        private Point EaLoginButton { get; set; }

        private Point MinBidText { get; set; }

        private Point MinBuyText { get; set; }

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;

        private const int MOUSEEVENTF_LEFTUP = 0x04;

        #endregion Properties

        #region Unmanaged Code

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        #endregion Unmanaged Code

        #region Constructors

        public MouseController(RunMode mode)
        {
            Mode = mode;
            LoadCalibrationSettings();
        }

        #endregion Constructors

        #region Public Methods

        public void PerformButtonClick(ButtonTypes buttonType)
        {
            switch (buttonType)
            {
                case ButtonTypes.Login:
                    DoClickAtPosition(LoginCoordinates.X, LoginCoordinates.Y);
                    break;
                case ButtonTypes.TransferNavigate:
                    DoClickAtPosition(TransferNavigateCoordinates.X, TransferNavigateCoordinates.Y);
                    break;
                case ButtonTypes.SearchTransferMarket:
                    DoClickAtPosition(SearchTransferMarketCoordinates.X, SearchTransferMarketCoordinates.Y);
                    break;
                case ButtonTypes.PlayerNameTextbox:
                    DoClickAtPosition(PlayerNameTextboxCoordinates.X, PlayerNameTextboxCoordinates.Y);
                    break;
                case ButtonTypes.PlayerNameTextDrop:
                    DoClickAtPosition(PlayerNameTextDropCoordinates.X, PlayerNameTextDropCoordinates.Y);
                    break;
                case ButtonTypes.MaxPriceTxt:
                    DoClickAtPosition(MaxPriceTextCoordinates.X, MaxPriceTextCoordinates.Y);
                    break;
                case ButtonTypes.IncreaseMinConsumable:
                    DoClickAtPosition(IncreaseMinConsumables.X, IncreaseMinConsumables.Y);
                    break;
                case ButtonTypes.DecreaseMinConsumable:
                    DoClickAtPosition(DecreaseMinConsumables.X, DecreaseMinConsumables.Y);
                    break;
                case ButtonTypes.Search:
                    DoClickAtPosition(SearchCoordinates.X, SearchCoordinates.Y);
                    break;
                case ButtonTypes.BuyNow:
                    DoClickAtPosition(BuyCoordinates.X, BuyCoordinates.Y);
                    break;
                case ButtonTypes.ConfirmPurchase:
                    DoClickAtPosition(ConfirmCoordinates.X, ConfirmCoordinates.Y);
                    break;
                case ButtonTypes.SendToTransferList:
                    DoClickAtPosition(SendToTransferListCoordinates.X, SendToTransferListCoordinates.Y);
                    break;
                case ButtonTypes.BackButton:
                    DoClickAtPosition(BackCoordinates.X, BackCoordinates.Y);
                    break;
                case ButtonTypes.ListItemBegin:
                    DoClickAtPosition(ListItemBegin.X, ListItemBegin.Y);
                    break;
                case ButtonTypes.ListMinTxt:
                    DoClickAtPosition(ListMinText.X, ListMinText.Y);
                    break;
                case ButtonTypes.ListMaxTxt:
                    DoClickAtPosition(ListMaxText.X, ListMaxText.Y);
                    break;
                case ButtonTypes.ListItemFinal:
                    DoClickAtPosition(ListItemFinal.X, ListItemFinal.Y);
                    break;
                case ButtonTypes.EaLogIn:
                    DoClickAtPosition(EaLoginButton.X, EaLoginButton.Y);
                    break;
                case ButtonTypes.MinBidText:
                    DoClickAtPosition(MinBidText.X, MinBidText.Y);
                    break;
                case ButtonTypes.MinBuyText:
                    DoClickAtPosition(MinBuyText.X, MinBuyText.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null);
            }
        }

        public void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }

        #endregion Public Methods

        #region Private Methods

        private void DoClickAtPosition(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private void LoadCalibrationSettings()
        {
            var loginX = Convert.ToInt32(ConfigurationManager.AppSettings["Login-X"]);
            var loginY = Convert.ToInt32(ConfigurationManager.AppSettings["Login-Y"]);
            var transfersNavigateX = Convert.ToInt32(ConfigurationManager.AppSettings["TransfersNavigate-X"]);
            var transfersNavigateY = Convert.ToInt32(ConfigurationManager.AppSettings["TransfersNavigate-Y"]);
            var searchTransferMarketX = Convert.ToInt32(ConfigurationManager.AppSettings["SearchTransferMarket-X"]);
            var searchTransferMarketY = Convert.ToInt32(ConfigurationManager.AppSettings["SearchTransferMarket-Y"]);
            var txtPlayerSearchX = Convert.ToInt32(ConfigurationManager.AppSettings["TxtPlayerName-X"]);
            var txtPlayerSearchY = Convert.ToInt32(ConfigurationManager.AppSettings["TxtPlayerName-Y"]);
            var txtPlayerDropX = Convert.ToInt32(ConfigurationManager.AppSettings["TxtPlayerNameDrop-X"]);
            var txtPlayerDropY = Convert.ToInt32(ConfigurationManager.AppSettings["TxtPlayerNameDrop-Y"]);
            var txtMaxPriceX = Convert.ToInt32(ConfigurationManager.AppSettings["TxtMaxPrice-X"]);
            var txtMaxPriceY = Convert.ToInt32(ConfigurationManager.AppSettings["TxtMaxPrice-Y"]);
            var searchX = Convert.ToInt32(ConfigurationManager.AppSettings["Search-X"]);
            var searchY = Convert.ToInt32(ConfigurationManager.AppSettings["Search-Y"]);
            var buyX = Convert.ToInt32(ConfigurationManager.AppSettings["Buy-X"]);
            var buyY = Convert.ToInt32(ConfigurationManager.AppSettings["Buy-Y"]);
            var confirmX = Convert.ToInt32(ConfigurationManager.AppSettings["Confirm-X"]);
            var confirmY = Convert.ToInt32(ConfigurationManager.AppSettings["Confirm-Y"]);
            var sendToTransferX = Convert.ToInt32(ConfigurationManager.AppSettings["SendTransfer-X"]);
            var sendToTransferY = Convert.ToInt32(ConfigurationManager.AppSettings["SendTransfer-Y"]);
            var backX = Convert.ToInt32(ConfigurationManager.AppSettings["Back-X"]);
            var backY = Convert.ToInt32(ConfigurationManager.AppSettings["Back-Y"]);
            var increaseConsX = Convert.ToInt32(ConfigurationManager.AppSettings["IncreaseConsumable-X"]);
            var increaseConsY = Convert.ToInt32(ConfigurationManager.AppSettings["IncreaseConsumable-Y"]);
            var decreaseConsX = Convert.ToInt32(ConfigurationManager.AppSettings["DecreaseConsumable-X"]);
            var decreaseConsY = Convert.ToInt32(ConfigurationManager.AppSettings["DecreaseConsumable-Y"]);
            var listItemBeginX = Convert.ToInt32(ConfigurationManager.AppSettings["ListTransferButton-X"]);
            var listItemBeginY = Convert.ToInt32(ConfigurationManager.AppSettings["ListTransferButton-Y"]);
            var listMinTxtX = Convert.ToInt32(ConfigurationManager.AppSettings["TxtListMin-X"]);
            var listMinTxtY = Convert.ToInt32(ConfigurationManager.AppSettings["TxtListMin-Y"]);
            var listMaxTxtX = Convert.ToInt32(ConfigurationManager.AppSettings["TxtListMax-X"]);
            var listMaxTxtY = Convert.ToInt32(ConfigurationManager.AppSettings["TxtListMax-Y"]);
            var listItemX = Convert.ToInt32(ConfigurationManager.AppSettings["ListItem-X"]);
            var listItemY = Convert.ToInt32(ConfigurationManager.AppSettings["ListItem-Y"]);
            var eaLoginX = Convert.ToInt32(ConfigurationManager.AppSettings["EaLogIn-X"]);
            var eaLoginY = Convert.ToInt32(ConfigurationManager.AppSettings["EaLogIn-Y"]);
            var minBidX = Convert.ToInt32(ConfigurationManager.AppSettings["MinBidTxt-X"]);
            var minBidY = Convert.ToInt32(ConfigurationManager.AppSettings["MinBidTxt-Y"]);
            var minBuyX = Convert.ToInt32(ConfigurationManager.AppSettings["MinBuyTxt-X"]);
            var minBuyY = Convert.ToInt32(ConfigurationManager.AppSettings["MinBuyTxt-Y"]);

            LoginCoordinates = new Point(loginX, loginY);
            TransferNavigateCoordinates = new Point(transfersNavigateX, transfersNavigateY);
            SearchTransferMarketCoordinates = new Point(searchTransferMarketX, searchTransferMarketY);
            PlayerNameTextboxCoordinates = new Point(txtPlayerSearchX, txtPlayerSearchY);
            PlayerNameTextDropCoordinates = new Point(txtPlayerDropX, txtPlayerDropY);
            MaxPriceTextCoordinates = new Point(txtMaxPriceX, txtMaxPriceY);
            SearchCoordinates = new Point(searchX, searchY);
            BuyCoordinates = new Point(buyX, buyY);
            ConfirmCoordinates = new Point(confirmX, confirmY);
            SendToTransferListCoordinates = new Point(sendToTransferX, sendToTransferY);
            BackCoordinates = new Point(backX, backY);
            IncreaseMinConsumables = new Point(increaseConsX, increaseConsY);
            DecreaseMinConsumables = new Point(decreaseConsX, decreaseConsY);
            ListItemBegin = new Point(listItemBeginX, listItemBeginY);
            ListMinText = new Point(listMinTxtX, listMinTxtY);
            ListMaxText = new Point(listMaxTxtX, listMaxTxtY);
            ListItemFinal = new Point(listItemX, listItemY);
            EaLoginButton = new Point(eaLoginX, eaLoginY);
            MinBidText = new Point(minBidX, minBidY);
            MinBuyText = new Point(minBuyX, minBuyY);
        }

        #endregion Private Methods
    }
}