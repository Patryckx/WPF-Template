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

        private string _DAQHost;

        public string DAQHost
        {

            get => _DAQHost;
            set{ _DAQHost = value;OnPropertyChanged(); }

        }

        private string _DatabaseAddress;

        public string DatabaseAddress
        {
            get => _DatabaseAddress;

            set { _DatabaseAddress = value;OnPropertyChanged(); }
        

        }

        private string _DMMPort;

        public string DMMPort
        {
            get => _DMMPort;

            set { _DMMPort = value; OnPropertyChanged(); }


        }


        private string _VisionCameraAddress;

        public string VisionCameraAddress
        {
            get => _VisionCameraAddress;

            set { _VisionCameraAddress = value; OnPropertyChanged(); }


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
            DAQHost = _config.DAQHost;
            DatabaseAddress = _config.DatabaseAddress;
            DMMPort = _config.DMM_port;
            VisionCameraAddress = _config.Camera_address;

          
        }

        private void Save()
        {
            _config.DAQHost = DAQHost;
            _config.DatabaseAddress = DatabaseAddress;
            _config.DMM_port = DMMPort;
            _config.Camera_address = VisionCameraAddress;   


            ReturnToReadScreen?.Invoke();

        }






    }
}
