using Example.Models;
using Example.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services
{
    public class AppStateService : IAppStateService ,INotifyPropertyChanged
    {
        private AppStatus _status;

        public AppStatus Status 
        {
            get => _status;
            set
            {
                _status = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(Status)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
