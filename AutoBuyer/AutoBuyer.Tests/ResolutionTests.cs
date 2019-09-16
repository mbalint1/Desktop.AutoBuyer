using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoBuyer.Tests
{
    [TestClass]
    public class ResolutionTests
    {
        [TestMethod]
        public void DoResolutionAdjustmentTest()
        {
            var resolution = Screen.PrimaryScreen.Bounds;
        }
    }
}