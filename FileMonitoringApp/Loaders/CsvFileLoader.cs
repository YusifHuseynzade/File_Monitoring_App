using Domain;
using Domain.Models;
using OfficeOpenXml; 

namespace Loaders
{
    public class CsvFileLoader : IFileLoader
    {
        public bool CanLoad(string filePath)
        {
            return filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ||
                   filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase); // ".xlsx teleblerde qeyd olunmasa bele ozum ekstra olaraq elave etdim."
        }

        public async Task<List<Trade>> LoadAsync(string filePath)
        {
            var trades = new List<Trade>();

           
            if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                await WaitForFileAccess(filePath);
                var lines = await File.ReadAllLinesAsync(filePath);
                for (int i = 1; i < lines.Length; i++) 
                {
                    var parts = lines[i].Split(',');
                    if (parts.Length == 6)
                    {
                        try
                        {
                            trades.Add(new Trade
                            {
                                Date = DateTime.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                                Open = decimal.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                                High = decimal.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture),
                                Low = decimal.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture),
                                Close = decimal.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture),
                                Volume = long.Parse(parts[5])
                            });
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"Hatalı veri formatı satır {i + 1} için: {ex.Message}");
                        }
                    }
                }
            }
            else if (filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                await WaitForFileAccess(filePath);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial; 
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; 
                    if (worksheet == null) return trades;

                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++) 
                    {
                        try
                        {
                            trades.Add(new Trade
                            {
                                Date = worksheet.Cells[row, 1].GetValue<DateTime>(),
                                Open = worksheet.Cells[row, 2].GetValue<decimal>(),
                                High = worksheet.Cells[row, 3].GetValue<decimal>(),
                                Low = worksheet.Cells[row, 4].GetValue<decimal>(),
                                Close = worksheet.Cells[row, 5].GetValue<decimal>(),
                                Volume = worksheet.Cells[row, 6].GetValue<long>()
                            });
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine($"Hatalı veri formatı satır {row} için: {ex.Message}");
                        }
                        catch (InvalidCastException ex)
                        {
                            Console.WriteLine($"Veri türü dönüşüm hatası satır {row} için: {ex.Message}");
                        }
                    }
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
            throw new IOException($"Dosyaya erişim sağlanamadı {maxRetries} denemeden sonra: {filePath}");
        }
    }
}