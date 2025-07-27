using Core.Services;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace WpfApp.ViewModels 
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FileMonitoringService _monitoringService;

        public ObservableCollection<Trade> Trades { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(FileMonitoringService monitoringService)
        {
            _monitoringService = monitoringService ?? throw new ArgumentNullException(nameof(monitoringService));
            Trades = new ObservableCollection<Trade>();

            _monitoringService.NewDataLoaded += OnNewDataLoaded;
            _monitoringService.Start();
        }

        private void OnNewDataLoaded(List<Trade> newTrades)
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var trade in newTrades)
                {
                    Trades.Add(trade);
                }
            });
        }
    }
}