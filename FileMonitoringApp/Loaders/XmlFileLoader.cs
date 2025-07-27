using Domain;
using Domain.Models;
using System.Globalization;
using System.Xml.Linq;

namespace Loaders
{
    public class XmlFileLoader : IFileLoader
    {
        public bool CanLoad(string filePath) => filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);

        public async Task<List<Trade>> LoadAsync(string filePath)
        {
            await WaitForFileAccess(filePath);

            var trades = new List<Trade>();

            XDocument doc;
            using (var stream = File.OpenRead(filePath))
            {
                doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            }

            if (doc.Root == null)
            {
                return trades;
            }

            foreach (var element in doc.Root.Elements("value"))
            {
                try
                {
                    trades.Add(new Trade
                    {
                        Date = DateTime.Parse(element.Attribute("date")?.Value),
                        Open = decimal.Parse(element.Attribute("open")?.Value, CultureInfo.InvariantCulture),
                        High = decimal.Parse(element.Attribute("high")?.Value, CultureInfo.InvariantCulture),
                        Low = decimal.Parse(element.Attribute("low")?.Value, CultureInfo.InvariantCulture),
                        Close = decimal.Parse(element.Attribute("close")?.Value, CultureInfo.InvariantCulture),
                        Volume = long.Parse(element.Attribute("volume")?.Value)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Skipping invalid element in {filePath}: {element}. Error: {ex.Message}");
                }
            }

            return trades;
        }

        private async Task WaitForFileAccess(string filePath, int maxRetries = 5, int delayMilliseconds = 200)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        return;
                    }
                }
                catch (IOException)
                {
                    await Task.Delay(delayMilliseconds);
                }
            }
            throw new IOException($"Could not access file after {maxRetries} retries: {filePath}");
        }
    }
}