using ConfigIniLib;
using ConfigIniLib.interfaces;

using SqlUtilityLibrary;
using SqlUtilityLibrary.Models;
using SqlUtilityLibrary.Services;
using SqlUtilityLibrary.Interfaces;

using EthModbus.Services;

using Example.Views;
using Example.Models;
using Example.ViewModels.Base;
using Example.Services.Interfaces;
using Example.Services;

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
using System.Data;

using Microsoft.Extensions.Configuration;

namespace Example.ViewModels
{
    public class MainViewModel : ViewModelBase 
    {
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }

        public ICommand ShowFirstScreenCommand { get; }
        public ICommand ShowSecondScreenCommand { get; }
        public ICommand ShowInicializeScreenCommand { get; }
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
        private readonly IAppStateService _appState;

        public IAppStateService AppState => _appState;

        public MainViewModel(IAppStateService appState)
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            IConfigService config = new AppConfigService(configPath);

            _configReadVM = new ConfigurationScreenViewModel(config);
            _configEditVM = new ConfigurationScreenEditViewModel(config);
            _configEditVM.ReturnToReadScreen = ShowConfigurationScreen;

            MinimizeCommand = new RelayCommand(MinimizeWindow);
            MaximizeCommand = new RelayCommand(MaximizeWindow);
            CloseCommand = new RelayCommand(CloseWindow);
            ShowFirstScreenCommand = new RelayCommand(ShowFirstScreen);
            ShowSecondScreenCommand = new RelayCommand(ShowSecondScreen);
            ShowInicializeScreenCommand = new RelayCommand(ShowInicializeScreen);
            ShowconfigurationScreenCommand = new RelayCommand(ShowConfigurationScreen);
            ShowConfigScreenCommand = new RelayCommand(ShowConfigScreen);

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
            
            //DATABASE
            _database = new SqlDatabaseService(db_config);

            IRegisterService registerService = new RegisterService(_database);

            //APPSTATE
            _appState = appState;

            ConnectionVM = new ConnectionViewModel(new ModbusService(), 
                config, 
                registerService, 
                _appState);

            CurrentView = new FirstScreenViewModel(ConnectionVM);

            Inicialize();
        }

        private async void Inicialize()
        {
            bool is_db_conected = await _database.TestConnectionAsync();
            if (is_db_conected)
            {
                Console.WriteLine("Conexion correcta con base de datos");

                _appState.Status = AppStatus.Idle;
            }
            else
            {
                Console.WriteLine("Conexion fallida con base de datos");
                _appState.Status = AppStatus.Error;

            }
        }
        
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

        public void ShowFirstScreen()
        {
            CurrentView = new FirstScreenViewModel(ConnectionVM);
        }


        public void ShowSecondScreen()
        {
            CurrentView = new SecondScreenViewModel(ConnectionVM, _database, _appState);
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
        
        private void Block_gui()
        {
            
        }

    }
}
