using Domain.Models;

namespace Domain
{
    public interface IFileLoader
    {
        bool CanLoad(string filePath);
        Task<List<Trade>> LoadAsync(string filePath);
    }
}
