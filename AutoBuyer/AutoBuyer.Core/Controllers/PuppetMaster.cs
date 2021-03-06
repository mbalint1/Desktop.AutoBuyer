﻿using System;
using System.Configuration;
using System.Threading;
using System.Timers;
using AutoBuyer.Core.API;
using AutoBuyer.Core.Enums;
using Timer = System.Timers.Timer;
using AutoBuyer.Core.Interfaces;
using AutoBuyer.Core.Models;
using AutoBuyer.Core.Data;
using AutoBuyer.Data.DTO;
using Player = AutoBuyer.Core.Models.Player;

namespace AutoBuyer.Core.Controllers
{
    public class PuppetMaster : IPuppetMaster
    {
        #region Properties and Fields

        private bool _continueNavigation;

        private IScreenController ScreenController { get; }

        private IMouseController MouseController { get; }

        private IKeyboardController KeyboardController { get; }

        private ILogger Logger { get; }

        private ISearchObject SearchObject { get; }

        private int MinPrice { get; set; }

        private Timer SearchLoadingTimer { get; set; }

        private Timer CaptchaMonitorTimer { get; set; }

        private bool KillSearchLoadingLoop { get; set; }

        public bool ProcessingInterrupted { get; set; }

        public InterruptScreen CurrentInterrupt { get; set; }

        private int PurchaseLoopIterations { get; }

        private int MsBetweenPurchaseClicks { get; }

        private bool AutoRecover { get; set; }

        private bool PauseInterruptChecks { get; set; }

        private ApiProvider API { get; set; }

        #endregion Properties and Fields

        #region Constructors

        public PuppetMaster(IScreenController screenController, ISearchObject searchObject, ILogger logger, bool autoRecover = false)
        {
            ScreenController = screenController;
            MouseController = new MouseController(searchObject.Mode);
            KeyboardController = new KeyboardController();
            Logger = logger;
            SearchObject = searchObject;
            SearchLoadingTimer = new Timer();
            SearchLoadingTimer.Elapsed += SearchTimerOnElapsed;
            SearchLoadingTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["SearchingMarketWaitTime"]);
            CaptchaMonitorTimer = new Timer();
            CaptchaMonitorTimer.Elapsed += CaptchaMonitorTimerOnElapsed;
            CaptchaMonitorTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["CaptchaMonitorWaitTime"]);
            ProcessingInterrupted = false;
            CurrentInterrupt = InterruptScreen.None;
            PurchaseLoopIterations = Convert.ToInt32(ConfigurationManager.AppSettings["purchaseLoopIterations"]);
            MsBetweenPurchaseClicks = Convert.ToInt32(ConfigurationManager.AppSettings["purchaseLoopMsBetweenClicks"]);
            AutoRecover = autoRecover;
            API = new ApiProvider();
        }

        #endregion Constructors

        #region Public Methods

        public void NavigateToTransferSearch()
        {
            _continueNavigation = true;

            while (_continueNavigation)
            {
                var screen = ScreenController.WhatScreenAmIOn();
                DoTransferSearchNavigation(screen);
            }
        }

        public void SetSearchParameters()
        {
            switch (SearchObject.Mode)
            {
                case RunMode.Player:
                    var player = (Player)SearchObject;
                    MouseController.PerformButtonClick(ButtonTypes.PlayerNameTextbox);
                    Thread.Sleep(1500);
                    KeyboardController.SendInput(player.Name);
                    Thread.Sleep(2200);
                    MouseController.PerformButtonClick(ButtonTypes.PlayerNameTextDrop);
                    Thread.Sleep(1500);
                    MouseController.PerformButtonClick(ButtonTypes.MaxPriceTxt);
                    Thread.Sleep(1500);
                    KeyboardController.SendInput(player.MaxPurchasePrice);
                    return;
                case RunMode.Consumable:
                    var consumable = (Consumable)SearchObject;
                    MouseController.PerformButtonClick(ButtonTypes.MaxPriceTxt);
                    Thread.Sleep(1500);
                    KeyboardController.SendInput(consumable.MaxPurchasePrice);
                    return;
            }
        }

        public void BuyPlayers()
        {
            Thread.Sleep(5000);

            var player = (Player)SearchObject;
            var maxPrice = Convert.ToInt32(ConfigurationManager.AppSettings["maxBuyLoop"]);
            var currentMinBid = 0;
            var currentMinBuy = 0;

            CaptchaMonitorTimer.Start();
            while (CurrentSession.Current.PurchasedNum < CurrentSession.Current.SearchNum)
            {
                if (ProcessingInterrupted)
                {
                    var canContinue = TryRecoverFromInterrupt();

                    if (!canContinue)
                    {
                        const int nestorWants3Emails = 3;

                        for (int i = 0; i < nestorWants3Emails; i++)
                        {
                            Thread.Sleep(20000);
                            API.SendMessage("Processing Interrupted", "Autobuyer needs your attention. There may be a captcha to solve.");
                        }

                        CurrentSession.Current.Captcha = true;
                        API.EndSession();

                        CaptchaMonitorTimer.Dispose();
                        return;
                    }
                }

                var succesfulSearch = false;
                
                //TODO: Remove this now that we are typing filters?
                Thread.Sleep(1500);

                for (int i = 0; i < 5; i++) // try to get a fresh filter set a max of 5 times, if it produces the same numbers 5 times, eh...
                {
                    var randoFilterPrices = GetFilterPrices(maxPrice);

                    if (randoFilterPrices.Item1 != currentMinBid && randoFilterPrices.Item2 != currentMinBuy)
                    {
                        currentMinBid = randoFilterPrices.Item1;
                        currentMinBuy = randoFilterPrices.Item2;
                        break;
                    }
                }

                SetFilterPrices(currentMinBid, currentMinBuy);

                MouseController.PerformButtonClick(ButtonTypes.Search);

                SearchLoadingTimer.Start();
                while (!KillSearchLoadingLoop)
                {
                    if (ScreenController.SuccessfulSearch())
                    {
                        succesfulSearch = true;
                        break;
                    }
                }
                SearchLoadingTimer.Stop();
                KillSearchLoadingLoop = false;

                if (succesfulSearch)
                {
                    DoPurchaseClicking(PurchaseLoopIterations, MsBetweenPurchaseClicks);

                    Thread.Sleep(2000);
                    MouseController.PerformButtonClick(ButtonTypes.ConfirmPurchase); // For some reason the confirmation box is sticking around
                    Thread.Sleep(4000); // Finalizing purchase

                    if (ScreenController.SuccessfulPurchase())
                    {
                        Thread.Sleep(2000);

                        if (player.AutoSell)
                        {
                            ListOnTransferMarket(player.SellMin, player.SellMax);
                        }
                        else
                        {
                            MouseController.PerformButtonClick(ButtonTypes.SendToTransferList);
                        }

                        CurrentSession.Current.PurchasedNum++;

                        var transaction = new TransactionLog
                        {
                            Type = TransactionType.SuccessfulPurchase,
                            TransactionDate = DateTime.Now,
                            PlayerName = player.Name,
                            SearchPrice = Convert.ToInt32(player.MaxPurchasePrice),
                            SellPrice = player.SellMax > 0 ? (int?) player.SellMax : null
                        };

                        System.Threading.Tasks.Task.Run(() => API.InsertTransactionLog(transaction, CurrentSession.Current.AccessToken));

                        Thread.Sleep(4000);
                    }
                    else
                    {
                        var transaction = new TransactionLog
                        {
                            Type = TransactionType.FailedPurchase,
                            TransactionDate = DateTime.Now,
                            PlayerName = player.Name,
                            SearchPrice = Convert.ToInt32(player.MaxPurchasePrice)
                        };

                        System.Threading.Tasks.Task.Run(() => API.InsertTransactionLog(transaction, CurrentSession.Current.AccessToken));

                        Thread.Sleep(500);
                    }
                }

                MouseController.PerformButtonClick(ButtonTypes.BackButton);
            }

            CaptchaMonitorTimer.Dispose();
            API.SendMessage("Run Complete", "We done here, yo");
            API.EndSession();
        }

        public void BuyConsumables()
        {
            //TODO: Broken, revisit later

            //Thread.Sleep(8000);
            //var decreasing = false;
            //var consumable = (Consumable)SearchObject;

            //CaptchaMonitorTimer.Start();

            //MouseController.PerformButtonClick(ButtonTypes.IncreaseMinConsumable);
            //while (NumberPurchased < consumable.NumberToPurchase)
            //{
            //    if (ProcessingInterrupted)
            //    {
            //        //TODO: Figure out how we are handling this. Email, mobile app notification, click OK, sign back in, solve captcha, etc.
            //        return;
            //    }

            //    var succesfulSearch = false;
            //    Thread.Sleep(1200);

            //    if (!decreasing)
            //    {
            //        MouseController.PerformButtonClick(ButtonTypes.IncreaseMinConsumable);
            //        decreasing = true;
            //    }
            //    else
            //    {
            //        MouseController.PerformButtonClick(ButtonTypes.DecreaseMinConsumable);
            //        decreasing = false;
            //    }

            //    Thread.Sleep(1200);
            //    MouseController.PerformButtonClick(ButtonTypes.Search);

            //    SearchLoadingTimer.Start();
            //    while (!KillSearchLoadingLoop)
            //    {
            //        if (ScreenController.SuccessfulSearch())
            //        {
            //            succesfulSearch = true;
            //            break;
            //        }
            //    }
            //    SearchLoadingTimer.Stop();
            //    KillSearchLoadingLoop = false;

            //    if (succesfulSearch)
            //    {
            //        DoPurchaseClicking(PurchaseLoopIterations, MsBetweenPurchaseClicks);

            //        Thread.Sleep(5000); // Finalizing purchase

            //        if (ScreenController.SuccessfulPurchase())
            //        {
            //            Thread.Sleep(2000);

            //            if (consumable.AutoSell)
            //            {
            //                ListOnTransferMarket(consumable.SellMin, consumable.SellMax);
            //            }
            //            else
            //            {
            //                MouseController.PerformButtonClick(ButtonTypes.SendToTransferList);
            //            }

            //            NumberPurchased++;
            //            System.Threading.Tasks.Task.Run(() => Logger.Log(LogType.Info, $"Consumable purchased successfully!"));
            //            Thread.Sleep(4000);
            //        }
            //        else
            //        {
            //            MouseController.PerformButtonClick(ButtonTypes.OutbidMessageBox);
            //            System.Threading.Tasks.Task.Run(() => Logger.Log(LogType.Info, $"Consumable failed to purchase."));
            //            Thread.Sleep(500);
            //        }
            //    }

            //    MouseController.PerformButtonClick(ButtonTypes.BackButton);
            //}
            //CaptchaMonitorTimer.Stop();

            //new MessageController().SendEmail("Run Complete", "We done here, yo");
        }

        #endregion Public Methods

        #region Private Methods

        private void DoPurchaseClicking(int numAttempts, int msBetweenClicks)
        {
            for (int i = 0; i < numAttempts; i++)
            {
                Thread.Sleep(msBetweenClicks);
                MouseController.PerformButtonClick(ButtonTypes.BuyNow);
                Thread.Sleep(msBetweenClicks);
                MouseController.PerformButtonClick(ButtonTypes.ConfirmPurchase);
            }
        }

        private void DoTransferSearchNavigation(Screens screen)
        {
            switch (screen)
            {
                case Screens.Login:
                    MouseController.PerformButtonClick(ButtonTypes.Login);
                    Thread.Sleep(20000);
                    break;
                case Screens.Home:
                    MouseController.PerformButtonClick(ButtonTypes.TransferNavigate);
                    Thread.Sleep(5000);
                    break;
                case Screens.TransferHome:
                    MouseController.PerformButtonClick(ButtonTypes.SearchTransferMarket);
                    Thread.Sleep(5000);
                    break;
                case Screens.TransferSearch:
                    _continueNavigation = false;
                    break;
                case Screens.Inconclusive:
                    break;
                case Screens.TransferResults:
                    //This should never happen
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ListOnTransferMarket(int minPrice, int maxPrice)
        {
            MouseController.PerformButtonClick(ButtonTypes.ListItemBegin);

            Thread.Sleep(2000);

            MouseController.PerformButtonClick(ButtonTypes.ListMinTxt);
            Thread.Sleep(1500);
            KeyboardController.SendInput(minPrice.ToString());

            Thread.Sleep(1000);

            MouseController.PerformButtonClick(ButtonTypes.ListMaxTxt);
            Thread.Sleep(1500);
            KeyboardController.SendInput(maxPrice.ToString());

            Thread.Sleep(1000);

            MouseController.PerformButtonClick(ButtonTypes.ListItemFinal);
        }

        private bool LogBackIn()
        {
            var successfulRecover = false;

            Thread.Sleep(2000); // Make sure button is active
            MouseController.PerformButtonClick(ButtonTypes.Login);
            Thread.Sleep(30000); // This can take awhile to load

            var currentScreen = ScreenController.WhatScreenAmIOn();

            if (currentScreen == Screens.EaSignIn)
            {
                Thread.Sleep(2000); // Make sure button is active
                MouseController.PerformButtonClick(ButtonTypes.EaLogIn);
                Thread.Sleep(30000); // This can take awhile to load
            }

            currentScreen = ScreenController.WhatScreenAmIOn();

            if (currentScreen == Screens.Home)
            {
                NavigateToTransferSearch();
                SetSearchParameters();
                successfulRecover = true;
            }

            return successfulRecover;
        }

        private bool TryRecoverFromInterrupt()
        {
            bool canContinue = false;

            if (AutoRecover)
            {
                if (CurrentInterrupt == InterruptScreen.Login)
                {
                    PauseInterruptChecks = true;

                    var recovered = LogBackIn();

                    if (recovered)
                    {
                        ProcessingInterrupted = false;
                        Thread.Sleep(5000);
                        PauseInterruptChecks = false;

                        canContinue = true;
                    }
                }
            }
            return canContinue;
        }

        private Tuple<int, int> GetFilterPrices(int maxPrice)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            var next1 = random.Next(2, maxPrice / 100);
            var next2 = random.Next(2, maxPrice / 100);

            var halfOrNah = DateTime.Now.Ticks % 2 == 0;

            var first = next1 * 100;
            var second = next2 * 100;

            if (halfOrNah)
            {
                first += 50;
                //second += 50;
            }

            var minBid = Math.Min(first, second);
            var minBuy = Math.Max(first, second);

            return new Tuple<int, int>(minBid, minBuy);
        }

        private void SetFilterPrices(int minBid, int minBuy)
        {
            MouseController.PerformButtonClick(ButtonTypes.MinBidText);
            KeyboardController.SendInput(minBid.ToString(), 120);

            Thread.Sleep(200);

            MouseController.PerformButtonClick(ButtonTypes.MinBuyText);
            KeyboardController.SendInput(minBuy.ToString(), 120);
        }

        #endregion Private Methods

        #region Event Handlers

        private void SearchTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            KillSearchLoadingLoop = true;
        }

        private void CaptchaMonitorTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (!PauseInterruptChecks)
            {
                var firstInterrupt = ScreenController.IsProcessingInterrupted();
                InterruptScreen doubleCheckInterrupt;

                if (firstInterrupt == InterruptScreen.None)
                {
                    CurrentInterrupt = InterruptScreen.None;
                    return;
                }
                else
                {
                    // Double Check
                    Thread.Sleep(1000);

                    doubleCheckInterrupt = ScreenController.IsProcessingInterrupted();
                }

                if (firstInterrupt == doubleCheckInterrupt)
                {
                    ProcessingInterrupted = true;
                    CurrentInterrupt = doubleCheckInterrupt;

                    new ApiProvider().SendMessage($"{CurrentInterrupt.ToString()} Time", $"It's {CurrentInterrupt.ToString()} time, yo");
                }
            }
        }

        #endregion Event Handlers
    }
}