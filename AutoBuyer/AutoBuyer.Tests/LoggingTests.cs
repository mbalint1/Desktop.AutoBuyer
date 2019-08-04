using System;
using System.IO;
using AutoBuyer.Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoBuyer.Tests
{
    [TestClass]
    public class LoggingTests
    {
        [TestMethod]
        public void WriteLogTest()
        {
            const string testLogPath = @"D:\Fifa Logs\UnitTest.log";

            var timeBeforeLog = DateTime.Now.Ticks;

            new Logger().Log(LogType.Info, "Testing");

            var timeAfterLog = DateTime.Now.Ticks;

            var fileInfo = new FileInfo(testLogPath);

            Assert.IsTrue(fileInfo.Exists);
            Assert.IsTrue(timeBeforeLog < fileInfo.LastWriteTime.Ticks);
            Assert.IsTrue(timeAfterLog > fileInfo.LastWriteTime.Ticks);
        }
    }
}
