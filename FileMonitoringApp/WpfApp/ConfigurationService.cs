using Core;
using Microsoft.Extensions.Configuration;
using System;

namespace WpfApp
{
    public class ConfigurationService
    {
        public MonitoringSettings GetSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            return configuration.GetSection("MonitoringSettings").Get<MonitoringSettings>();
        }
    }
}
