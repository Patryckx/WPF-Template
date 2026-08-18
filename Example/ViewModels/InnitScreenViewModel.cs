using Example.Models;
using Example.Navigation.Services;
using Example.Services.Interfaces;
using Example.ViewModels.Base;
using System.Windows;
using System.Windows.Navigation;

namespace Example.ViewModels
{
    public class InnitScreenViewModel : ViewModelBase
    {
        private readonly IStartupService _startupService;

        private readonly INavigationService _navigation;

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
            IStartupService startupService, INavigationService navigation)
        {
            _startupService = startupService;
            _navigation = navigation;
            _ = StartAsync();
        }


        private async Task StartAsync()
        {
            try
            {
                var progress = new Progress<StartupProgress>(value =>
                {
                    Progress = value.Progress;
                    StatusMessage = value.Message;
                });

                await _startupService.InitializeAsync(progress);


                _navigation.Navigate<DCVoltage_test_ViewModel>();

            }
            catch (Exception ex)
            {
                StatusMessage = "Error durante la inicialización.";

                // Temporalmente para depuración
                System.Windows.MessageBox.Show(
                    ex.ToString(),
                    "Error de inicialización");
            }
        }
    }
}