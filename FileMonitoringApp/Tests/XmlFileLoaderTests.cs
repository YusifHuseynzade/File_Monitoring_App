using Domain.Models;
using Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class XmlFileLoaderTests
    {
        [TestMethod]
        public async Task LoadAsync_ValidXmlFile_ReturnsTrades()
        {
            var loader = new XmlFileLoader();
            var xmlContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<values>
    <value date=""2013-05-20"" open=""30.16"" high=""30.39"" low=""30.02"" close=""30.17"" volume=""1478200"" />
</values>";
            var filePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(filePath, xmlContent);

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
        public void CanLoad_XmlFile_ReturnsTrue()
        {
            var loader = new XmlFileLoader();
            Assert.IsTrue(loader.CanLoad("test.xml"));
        }

        [TestMethod]
        public void CanLoad_NonXmlFile_ReturnsFalse()
        {
            var loader = new XmlFileLoader();
            Assert.IsFalse(loader.CanLoad("test.txt"));
        }
    }
}