using CsvHelper;
using CsvHelper.Configuration;
using Domain;
using Domain.Models;
using System.Globalization;

namespace Loaders
{
    public sealed class TxtFileLoader : IFileLoader
    {
        public bool CanLoad(string filePath) => filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);

        public async Task<List<Trade>> LoadAsync(string filePath)
        {
            await WaitForFileAccess(filePath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ";",
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<Trade>();
            return records.ToList();
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