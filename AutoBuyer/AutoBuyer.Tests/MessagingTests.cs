using System.Configuration;
using System.IO;
using AutoBuyer.Core.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoBuyer.Tests
{
    [TestClass]
    public class MessagingTests
    {
        [TestMethod]
        public void ReadConfigTest()
        {
            var stuffs = File.ReadAllText(ConfigurationManager.AppSettings["pFile"]).Trim().Split(',');

            Assert.IsTrue(stuffs.Length >= 3);
            Assert.IsTrue(stuffs[1].Contains("@"));
            Assert.IsTrue(stuffs[2].Contains("@"));
        }

        [TestMethod]
        public void SendEmailTest()
        {
            new MessageController().SendEmail("Testing", "Test message, yo");
        }
    }
}