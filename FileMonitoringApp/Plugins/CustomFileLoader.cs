using Domain;
using Domain.Models;

namespace Plugins
{
    public class CustomFileLoader : IFileLoader
    {
        public bool CanLoad(string filePath) => filePath.EndsWith(".custom", StringComparison.OrdinalIgnoreCase);

        public Task<List<Trade>> LoadAsync(string filePath)
        {
            var trades = new List<Trade>();
            return Task.FromResult(trades);
        }
    }
}