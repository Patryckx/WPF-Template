using Example.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services
{
    public class StartupService :IStartupService,INotifyPropertyChanged
    {
        public double _progress;

        public double Progress
        {
            get => _progress;

            private set
            {
                _progress = value;
                OnPropertyChanged();

            }
        }

        public string _statusMessage = string.Empty;

        public string StatusMessage
        {
            get => _statusMessage;

            private set
            {
                _statusMessage= value;

                OnPropertyChanged();
            }

        }


        public async Task StartAsync()
        {
            Progress = 0;

            StatusMessage = "Inicializando aplicacion...";


            await Task.Delay(500);

            Progress = 25;
            StatusMessage = "Comprobando base de datos...";


            await Task.Delay(500);


            Progress = 75;
            StatusMessage="Preparando comunicacion con PLC";

            await Task.Delay(500);


            Progress = 100;

            StatusMessage = "Inicialización completa";

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }

    }
}
