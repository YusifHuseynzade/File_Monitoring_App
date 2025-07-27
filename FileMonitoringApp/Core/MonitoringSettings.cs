namespace Core
{
    public class MonitoringSettings
    {
        public string InputDirectory { get; set; }
        public int MonitoringFrequencySeconds { get; set; }
        public List<string> LoaderAssemblies { get; set; }
    }
}
