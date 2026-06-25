using ConfigIniLib;
using ConfigIniLib.interfaces;
using EthModbus.Services;

using Example.Models;
using Example.ViewModels.Base;
using Example.Views;
using SqlUtilityLibrary;
using SqlUtilityLibrary.Services;
using SqlUtilityLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using SqlUtilityLibrary.Models;
using Example.Services.Interfaces;
using Example.Services;
using System.Data;





namespace Example.ViewModels
{
    public class MainViewModel : ViewModelBase 
    {
        /// WINDOW FUNCTIONS
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }
        /// VIEWS NAVIGATION FUNCTIONS
        public ICommand ShowFirstScreenCommand { get; }
        public ICommand ShowSecondScreenCommand { get; }
        public ICommand ShowInicializeScreenCommand { get; }
        //Configuration
        public ICommand ShowconfigurationScreenCommand { get; }
        public ICommand ShowConfigurationEditScreenCommand { get; }

        public ICommand ShowConfigScreenCommand {  get; }


        private object? _currentView;

        public object? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value; 
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        //****************************************
        //  CONNECTION VIEWMODEL
        //****************************************

        private ConnectionViewModel _connectionVM;

        public ConnectionViewModel ConnectionVM
        {
            get => _connectionVM;

            set     
            {
                _connectionVM = value;
                OnPropertyChanged();
            }

        }

        //config screens
        private readonly ConfigurationScreenViewModel _configReadVM;
        private readonly ConfigurationScreenEditViewModel _configEditVM;


        private readonly IDataService _database;




        //****************************************
        //  CONSTRUCTOR
        //****************************************

        public MainViewModel()
        {

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            IConfigService config = new AppConfigService(configPath);


            _configReadVM = new ConfigurationScreenViewModel(config);

            _configEditVM = new ConfigurationScreenEditViewModel(config);

            _configEditVM.ReturnToReadScreen = ShowConfigurationScreen;

            CurrentView = new FirstScreenViewModel(ConnectionVM);

            MinimizeCommand = new RelayCommand(MinimizeWindow);
            MaximizeCommand = new RelayCommand(MaximizeWindow);
            CloseCommand = new RelayCommand(CloseWindow);

            ShowFirstScreenCommand = new RelayCommand(ShowFirstScreen);
            ShowSecondScreenCommand = new RelayCommand(ShowSecondScreen);
            ShowInicializeScreenCommand = new RelayCommand(ShowInicializeScreen);

            //Configuration
            ShowconfigurationScreenCommand = new RelayCommand(ShowConfigurationScreen);

            ShowConfigScreenCommand = new RelayCommand(ShowConfigScreen);

            //ShowConfigurationEditScreenCommand = new RelayCommand(ShowConfigurationEditScreen);

            //Database config
            IConfiguration configuration =
                new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("AppSettings.json")
                .Build();

            string? connectionString = configuration["Database_settings:ConnectionString"];


            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "No se encontro la cadena de conexion.");
            }

            DatabaseConfig db_config = new()
            {
                ConnectionString = connectionString,
            };
            
            
            _database = new SqlDatabaseService(db_config);

            IRegisterService registerService = new RegisterService(_database);

            ConnectionVM = new ConnectionViewModel(new ModbusService(), config, registerService);


            Inicialize();
        }

        private async void Inicialize()
        {
            bool is_db_conected = await _database.TestConnectionAsync();
            if (is_db_conected)
            {
                Console.WriteLine("Conexion correcta con base de datos");
            }
            else
            {
                Console.WriteLine("Conexion fallida con base de datos");
            }

        }


        //****************************************
        //  WINDOW FUNCTIONS
        //****************************************


        private void MinimizeWindow()
        {
            Application.Current.MainWindow.WindowState=WindowState.Minimized;
        }

        private void MaximizeWindow()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            else
                Application.Current.MainWindow.WindowState = WindowState.Maximized;

        }

        private void CloseWindow()
        {
            Application.Current.Shutdown();
        }

        //****************************************
        //    VIEW FUNCTIONS
        //****************************************

        public void ShowFirstScreen()
        {
            CurrentView = new FirstScreenViewModel(ConnectionVM);
        }


        public void ShowSecondScreen()
        {
            CurrentView = new SecondScreenViewModel();
        }

        public void ShowInicializeScreen()
        {
            CurrentView = new InicializeScreenViewModel();
        }




        public void ShowConfigurationScreen()

        {
            //CurrentView = new ConfigurationScreenViewModel();
            _configReadVM.Refresh();
            CurrentView = _configReadVM;
        }



        private void ShowConfigScreen()
        {
            CurrentView = _configEditVM;
        }



        
        //****************************************
        //    VIEW FUNCTIONS
        //****************************************

        private void Block_gui()
        {
            
        }

    }
}
