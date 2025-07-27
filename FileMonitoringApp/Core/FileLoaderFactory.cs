using Domain;
using Loaders;
using System.Reflection;

namespace Core
{
    public class FileLoaderFactory : IFileLoaderFactory
    {
        private readonly List<IFileLoader> _loaders;

        public FileLoaderFactory(MonitoringSettings settings)
        {
            _loaders = new List<IFileLoader>();


            _loaders.Add(new CsvFileLoader());
            _loaders.Add(new TxtFileLoader());
            _loaders.Add(new XmlFileLoader());


            foreach (var assemblyPath in settings.LoaderAssemblies)
            {
                var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyPath);
                if (File.Exists(fullPath))
                {
                    var assembly = Assembly.LoadFrom(fullPath);
                    var loaderTypes = assembly.GetTypes()
                        .Where(t => typeof(IFileLoader).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in loaderTypes)
                    {
                        var loader = Activator.CreateInstance(type) as IFileLoader;
                        if (loader != null)
                        {
                            _loaders.Add(loader);
                        }
                    }
                }
            }
        }

        public IFileLoader GetLoader(string filePath)
        {
            return _loaders.FirstOrDefault(loader => loader.CanLoad(filePath));
        }
    }
}