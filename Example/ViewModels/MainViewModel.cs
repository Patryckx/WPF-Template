using ConfigIniLib;
using ConfigIniLib.interfaces;
using EthModbus.Services;
using Example.Models;
using Example.ViewModels.Base;
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

            //****************************************
        //  CONSTRUCTOR
        //****************************************

        public MainViewModel()
        {

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            IConfigService config = new AppConfigService(configPath);


            ConnectionVM =new ConnectionViewModel(new ModbusService(), config);
            CurrentView = new FirstScreenViewModel(ConnectionVM);

            MinimizeCommand = new RelayCommand(MinimizeWindow);
            MaximizeCommand = new RelayCommand(MaximizeWindow);
            CloseCommand = new RelayCommand(CloseWindow);

            ShowFirstScreenCommand = new RelayCommand(ShowFirstScreen);
            ShowSecondScreenCommand = new RelayCommand(ShowSecondScreen);

            ShowInicializeScreenCommand = new RelayCommand(ShowInicializeScreen);

            //Configuration

           


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



    }
}
