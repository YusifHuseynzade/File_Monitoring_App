// WpfApp/App.xaml.cs

using Core;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var configService = new ConfigurationService();
            var settings = configService.GetSettings();
            services.AddSingleton(settings);

            services.AddSingleton<IFileLoaderFactory, FileLoaderFactory>();
            services.AddSingleton<FileMonitoringService>();

            services.AddSingleton<MainViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainViewModel = _serviceProvider.GetService<MainViewModel>();
            var mainWindow = new MainWindow(mainViewModel);
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}