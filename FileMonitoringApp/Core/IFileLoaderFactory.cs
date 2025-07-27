using Domain;

namespace Core
{
    public interface IFileLoaderFactory
    {
        IFileLoader GetLoader(string filePath);
    }
}
