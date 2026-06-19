using ConfigIniLib;
using ConfigIniLib.interfaces;
using EthModbus.Services;
using Example.Models;
using Example.ViewModels.Base;
using Example.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Example.ViewModels
{
    public class MainViewModel :ViewModelBase 
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

            ConnectionVM =new ConnectionViewModel(new ModbusService(), config);
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
