using Domain.Models;
using Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class TxtFileLoaderTests
    {
        [TestMethod]
        public async Task LoadAsync_ValidTxtFile_ReturnsTrades()
        {
            var loader = new TxtFileLoader();
            var txtContent = @"Date;Open;High;Low;Close;Volume
2013-05-20;30.16;30.39;30.02;30.17;1478200";
            var filePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(filePath, txtContent);

            var trades = await loader.LoadAsync(filePath);

            Assert.IsNotNull(trades);
            Assert.AreEqual(1, trades.Count);

            var trade = trades[0];
            Assert.AreEqual(new DateTime(2013, 5, 20), trade.Date);
            Assert.AreEqual(30.16m, trade.Open);
            Assert.AreEqual(30.39m, trade.High);
            Assert.AreEqual(30.02m, trade.Low);
            Assert.AreEqual(30.17m, trade.Close);
            Assert.AreEqual(1478200, trade.Volume);

            File.Delete(filePath);
        }

        [TestMethod]
        public void CanLoad_TxtFile_ReturnsTrue()
        {
            var loader = new TxtFileLoader();
            Assert.IsTrue(loader.CanLoad("test.txt"));
        }

        [TestMethod]
        public void CanLoad_NonTxtFile_ReturnsFalse()
        {
            var loader = new TxtFileLoader();
            Assert.IsFalse(loader.CanLoad("test.csv"));
        }
    }
}