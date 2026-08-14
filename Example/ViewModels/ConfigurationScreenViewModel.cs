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

        private string _DAQHost;
        public string DAQHost
        {
            get => _DAQHost;
            set
            {
                _DAQHost = value;
                OnPropertyChanged();
            }
        }

        private string _DatabaseAddress;
        public string DatabaseAddress
        {
            get => _DatabaseAddress;
            set
            {
                _DatabaseAddress = value;
                OnPropertyChanged();
            }
        }

        private string _DMMPort;
        public string DMMPort
        {
            get => _DMMPort;
            set
            {
                _DMMPort = value;
                OnPropertyChanged();
            }
        }


        private string _VisionCameraAddress;
        public string VisionCameraAddress
        {
            get => _VisionCameraAddress;
            set
            {
                _VisionCameraAddress = value;
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
            DAQHost = _config.DAQHost;
            DatabaseAddress = _config.DatabaseAddress;
            DMMPort=_config.DMM_port;
            VisionCameraAddress=_config.Camera_address;
        }

        public void Refresh()
        {
            Load();
        }
    }
}
