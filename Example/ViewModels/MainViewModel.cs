using Example.ViewModels.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Example.ViewModels
{
    public class MainViewModel :INotifyPropertyChanged
    {
        
        /// WINDOW FUNCTIONS
       
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }

        /// VIEWS NAVIGATION FUNCTIONS
        
        public ICommand ShowFirstScreenCommand { get; }
        public ICommand ShowSecondScreenCommand { get; }


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

        public MainViewModel()
        {
            CurrentView = new FirstScreenViewModel();

            MinimizeCommand = new RelayCommand(MinimizeWindow);
            MaximizeCommand = new RelayCommand(MaximizeWindow);
            CloseCommand = new RelayCommand(CloseWindow);

            ShowFirstScreenCommand = new RelayCommand(ShowFirstScreen);
            ShowSecondScreenCommand = new RelayCommand(ShowSecondScreen);

        }


        /// <summary>
        /// WINDOW FUNCTIONS
        /// </summary>


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





        /// VIEW FUNCTIONS
        
        public void ShowFirstScreen()
        {
            CurrentView = new FirstScreenViewModel();
        }


        public void ShowSecondScreen()
        {
            CurrentView = new SecondScreenViewModel();
        }








        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}
