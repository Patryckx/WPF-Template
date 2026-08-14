using Example.Models;
using Example.Services.Interfaces;
using Example.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    public class InnitScreenViewModel : ViewModelBase
    {
        private readonly IStartupService _startupService;

        public RelayCommand StartCommand { get; }

        private double _progress;

        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }


        private string _statusMessage = string.Empty;

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }


        public InnitScreenViewModel(
            IStartupService startupService)
        {
            _startupService = startupService;
            StartCommand = new RelayCommand(() => _ = StartAsync());
        }

        private async Task StartAsync()
        {
            var progress = new Progress<StartupProgress>(value =>
            {
                Progress = value.Progress;
                StatusMessage = value.Message;
            });

            await _startupService.InitializeAsync(progress);
        }
    }
}
