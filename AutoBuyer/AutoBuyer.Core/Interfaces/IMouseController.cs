using System;
using AutoBuyer.Core.Enums;

namespace AutoBuyer.Core.Interfaces
{
    interface IMouseController
    {
        RunMode Mode { get; }
        void PerformButtonClick(ButtonTypes buttonType);
        void BringConsoleToFront();
    }
}