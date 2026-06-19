using ConfigIniLib.interfaces;
using Example.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    public class ConfigurationScreenViewModel : ViewModelBase
    {
        private readonly IConfigService _config;

        private string _host;
        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                OnPropertyChanged();
            }
        }

        private int _puerto;
        public int Puerto
        {
            get => _puerto;
            set
            {
                _puerto = value;
                OnPropertyChanged();
            }
        }

        private string _direccionBD;
        public string DireccionBD
        {
            get => _direccionBD;
            set
            {
                _direccionBD = value;
                OnPropertyChanged();
            }
        }

        public ConfigurationScreenViewModel(IConfigService config)
        {
            _config = config;

            Load();
        }

        private void Load()
        {
            Host = _config.Host;
            Puerto = _config.Port;
            DireccionBD = _config.DatabaseAddress;
        }

        public void Refresh()
        {
            Load();
        }
    }
}
