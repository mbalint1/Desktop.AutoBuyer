using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing.Imaging;
using AutoBuyer.Core.Enums;
using AutoBuyer.Core.Interfaces;
using AutoBuyer.Core.Utilities;

namespace AutoBuyer.Core.Controllers
{
    public class ScreenController : IScreenController
    {
        #region Properties

        private string TrainingSetBaseFilePath => System.Configuration.ConfigurationManager.AppSettings["TrainingSetRoot"];

        private Process _browser;

        private Bitmap LoginTrainer { get; }

        private Bitmap HomeTrainer { get; }

        private Bitmap TransferHomeTrainer { get; }

        private Bitmap TransferSearchTrainer { get; }

        private Bitmap ResultWithPlayerTrainer { get; }

        private Bitmap SuccessfulPurchaseTrainer { get; }

        private Bitmap FailedPurchaseTrainer { get; }

        private Bitmap CaptchaMessageTrainer { get; }

        private Bitmap ServiceUnavailableTrainer { get; }

        private Bitmap EaLoginTrainer { get; }

        #endregion Properties

        #region Native Code

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        #endregion Native Code

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }

        #endregion Structs

        #region Constructors

        public ScreenController()
        {
            try
            {
                LoginTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}Login.png"));
                HomeTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}Home.png"));
                TransferHomeTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}TransfersHome.png"));
                TransferSearchTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}TransferSearch.png"));
                ResultWithPlayerTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}TransferResultsSuccess.png"));
                SuccessfulPurchaseTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}SuccessfulPurchase.png"));
                FailedPurchaseTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}FailedPurchase.png"));
                CaptchaMessageTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}CaptchaMessage.png"));
                ServiceUnavailableTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}ServiceUnavailable.png"));
                EaLoginTrainer = new Bitmap(Image.FromFile($@"{TrainingSetBaseFilePath}EaLogIn.png"));
            }
            catch (Exception ex)
            {
                //TODO: Add logging.
                throw;
            }
        }

        #endregion Constructors

        #region Public Methods

        public void OpenBrowser()
        {
            try
            {
                _browser = new Process { StartInfo = { FileName = ConfigurationManager.AppSettings["WebAppUrl"] } };
                _browser.Start();

                do
                {
                    Thread.Sleep(100);
                    _browser.Refresh();
                }
                while (_browser.MainWindowHandle == IntPtr.Zero && !_browser.HasExited);

                if (!_browser.HasExited)
                {
                    var screenBounds = Screen.PrimaryScreen.Bounds;
                    var x = screenBounds.Width / 2;
                    const int y = 0;
                    var width = screenBounds.Width / 2;
                    var height = screenBounds.Height - (screenBounds.Height / 10);
                    MoveWindow(_browser.MainWindowHandle, x, y, width, height, true);
                }
            }
            catch (Exception ex)
            {
                //TODO: Add logging
                throw;
            }
        }

        public Bitmap CaptureBrowser()
        {
            try
            {
                do
                {
                    Thread.Sleep(100);
                    _browser.Refresh();
                }
                while (_browser.MainWindowHandle == IntPtr.Zero && !_browser.HasExited);

                if (!_browser.HasExited)
                {
                    var ptr = _browser.MainWindowHandle;
                    var browserRect = new Rect();
                    GetWindowRect(ptr, ref browserRect);
                    var bounds = new Rectangle(browserRect.Left, browserRect.Top, browserRect.Right - browserRect.Left, browserRect.Bottom - browserRect.Top);

                    var result = new Bitmap(bounds.Width, bounds.Height);
                    using (var g = Graphics.FromImage(result))
                    {
                        g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                    }

                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                //TODO: Add logging
                throw;
            }
        }

        public Bitmap CaptureBrowserTransferResults()
        {
            try
            {
                if (!_browser.HasExited)
                {
                    var ptr = _browser.MainWindowHandle;
                    var browserRect = new Rect();
                    GetWindowRect(ptr, ref browserRect);
                    var offsetX = (browserRect.Right - browserRect.Left) / 3 * 2;
                    var offsetY = (browserRect.Bottom - browserRect.Top) / 6;
                    var x = (int)Math.Round((double)(browserRect.Left + offsetX), 0);
                    var y = (int)Math.Round((double)(browserRect.Top + offsetY), 0);
                    var width = (browserRect.Right - browserRect.Left) - offsetX;
                    var height = (browserRect.Bottom - browserRect.Top) - (offsetY * 3);

                    var bounds = new Rectangle(x, y, width, height);

                    var result = new Bitmap(bounds.Width, bounds.Height);
                    using (var g = Graphics.FromImage(result))
                    {
                        g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                    }

                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                //TODO: Add logging
                throw;
            }
        }

        public Bitmap CapturePurchaseResults()
        {
            try
            {
                if (!_browser.HasExited)
                {
                    var ptr = _browser.MainWindowHandle;
                    var browserRect = new Rect();
                    GetWindowRect(ptr, ref browserRect);
                    var offsetX = (browserRect.Right - browserRect.Left) / 3 * 2;
                    var offsetY = (browserRect.Bottom - browserRect.Top) / 2.5;
                    var x = (int)Math.Round((double)(browserRect.Left + offsetX), 0);
                    var y = (int)Math.Round((double)(browserRect.Top + offsetY), 0);
                    var width = (browserRect.Right - browserRect.Left) - offsetX;
                    var height = (browserRect.Bottom - browserRect.Top) / 10;

                    var bounds = new Rectangle(x, y, width, height);

                    var result = new Bitmap(bounds.Width, bounds.Height);
                    using (var g = Graphics.FromImage(result))
                    {
                        g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                    }

                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                //TODO: Add logging
                throw;
            }
        }

        public Bitmap CaptureCaptchaResults()
        {
            try
            {
                if (!_browser.HasExited)
                {
                    var ptr = _browser.MainWindowHandle;
                    var browserRect = new Rect();
                    GetWindowRect(ptr, ref browserRect);

                    var centerX = ((browserRect.Right - browserRect.Left) / 2) + browserRect.Left;
                    var centerY = (browserRect.Bottom - browserRect.Top) / 2;

                    var x = (int)Math.Round((double)centerX - (centerX / 6));
                    var y = (int)Math.Round((double)centerY - (centerY / 5));

                    var bounds = new Rectangle(x, y, 425, 300);

                    var result = new Bitmap(bounds.Width, bounds.Height);
                    using (var g = Graphics.FromImage(result))
                    {
                        g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                    }

                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                //TODO: Add logging
                throw;
            }
        }

        public bool SuccessfulSearch()
        {
            var imageworker = new ImageManipulator();

            var results = CaptureBrowserTransferResults();

            var percentMatch = imageworker.PercentMatch(results, ResultWithPlayerTrainer);

            results.Dispose();

            return percentMatch >= 90;
        }

        public bool SuccessfulPurchase()
        {
            var imageworker = new ImageManipulator();

            var purchaseCapture = CapturePurchaseResults();

            var successScore = imageworker.PercentMatch(purchaseCapture, SuccessfulPurchaseTrainer);
            var failureScore = imageworker.PercentMatch(purchaseCapture, FailedPurchaseTrainer);

            purchaseCapture.Dispose();

            return successScore > failureScore;
        }

        public InterruptScreen IsProcessingInterrupted()
        {
            var imageWorker = new ImageManipulator();

            var messageCapture = CaptureCaptchaResults();

            var captcha = imageWorker.PercentMatch(messageCapture, CaptchaMessageTrainer);
            var serviceUnavailable = imageWorker.PercentMatch(messageCapture, ServiceUnavailableTrainer);

            messageCapture.Dispose();

            if (captcha > 85)
            {
                return InterruptScreen.Captcha;
            }

            if (serviceUnavailable > 85)
            {
                return InterruptScreen.ServiceUnavailable;
            }

            if (WhatScreenAmIOn() == Screens.Login)
            {
                return InterruptScreen.Login;
            }

            return InterruptScreen.None;
        }

        public Screens WhatScreenAmIOn()
        {
            var current = CaptureBrowser();

            var imageWorker = new ImageManipulator();

            var login = imageWorker.PercentMatch(current, LoginTrainer);
            var home = imageWorker.PercentMatch(current, HomeTrainer);
            var transferHome = imageWorker.PercentMatch(current, TransferHomeTrainer);
            var transferSearch = imageWorker.PercentMatch(current, TransferSearchTrainer);
            var eaLogin = imageWorker.PercentMatch(current, EaLoginTrainer);
            current.Dispose();

            var dict = new Dictionary<Screens, decimal>
            {
                {Screens.Login, login},
                {Screens.Home, home},
                {Screens.TransferHome, transferHome},
                {Screens.TransferSearch, transferSearch},
                {Screens.EaSignIn, eaLogin }
            };

            var screen = dict.OrderByDescending(x => x.Value).FirstOrDefault().Key;

            return dict[screen] <= 65 ? Screens.Inconclusive : screen;
        }

        #endregion Public Methods
    }
}