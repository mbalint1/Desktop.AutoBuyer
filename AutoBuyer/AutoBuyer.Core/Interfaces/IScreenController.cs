using System.Drawing;
using AutoBuyer.Core.Enums;

namespace AutoBuyer.Core.Interfaces
{
    public interface IScreenController
    {
        void OpenBrowser();
        Bitmap CaptureBrowser();
        Bitmap CaptureBrowserTransferResults();
        Bitmap CapturePurchaseResults();
        Bitmap CaptureCaptchaResults();
        bool SuccessfulSearch();
        bool SuccessfulPurchase();
        InterruptScreen IsProcessingInterrupted();
        Screens WhatScreenAmIOn();
    }
}