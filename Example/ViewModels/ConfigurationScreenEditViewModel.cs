using ConfigIniLib;
using ConfigIniLib.interfaces;
using Example.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Example.ViewModels
{
    public class ConfigurationScreenEditViewModel : ViewModelBase
    {
        private readonly IConfigService _config;

        public Action? ReturnToReadScreen { get; set; }

        public RelayCommand SaveCommand { get;  }

        //Propiedades bindeadas a la UI

        private string _host;

        public string Host {

            get => _host;
            set{ _host = value;OnPropertyChanged(); }

        }

        public int _port;

        public int Port
        {
            get => _port;
            set { _port = value;OnPropertyChanged();  }
        }


        private string _DatabaseAddress;

        public string DatabaseAddress
        {
            get => _DatabaseAddress;

            set { _DatabaseAddress = value;OnPropertyChanged(); }
        

        }

        // ── Constructor ───────────────────────────────

        public ConfigurationScreenEditViewModel(IConfigService config)
        {
            _config = config;

            SaveCommand = new RelayCommand(Save);

            // Carga los valores actuales del .ini al abrir la pantalla
            Load();
        }

        private void Load()
        {
            Host = _config.Host;
            Port = _config.Port;
            DatabaseAddress = _config.DatabaseAddress;
          
        }

        private void Save()
        {
            _config.Host = Host;
            _config.Port = Port;
            _config.DatabaseAddress = DatabaseAddress;

            ReturnToReadScreen?.Invoke();

        }






    }
}
