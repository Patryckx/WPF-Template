using SqlUtilityLibrary.Interfaces;

using Example.Models;
using Example.ViewModels.Base;
using Example.Services.Interfaces;

using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Example.Navigation.Services;
using Example.Navigation.Stores;
using Example.Navigation.Commands;


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

        //DC VOLTAGE TEST 1
        private DCVoltage_test_ViewModel _DCVoltageVM;
        public DCVoltage_test_ViewModel DCVoltageVM
        {
            get => _DCVoltageVM;

            set
            {
                _DCVoltageVM = value;
                OnPropertyChanged();
            }
        }


        public ViewModelBase? CurrentView
        {
            get => _navigationStore.CurrentViewModel;
        }

        private readonly INavigationService _navigation;
        private readonly IDataService _database;
        private readonly IAppStateService _appState;
        private readonly NavigationStore _navigationStore;
        public IAppStateService AppState => _appState;

        private readonly ILoggerService _logger;

        public MainViewModel(
            INavigationService navigation,
            IAppStateService appState, 
            IDataService database,
            ConnectionViewModel connection,
            ConfigurationScreenViewModel configRead,
            ConfigurationScreenEditViewModel configEdit,
            NavigationStore navigationStore,
            ILoggerService logger
            )
        {

            _navigation = navigation;
            _appState = appState;
            _database = database;
            _navigationStore= navigationStore;
            _logger = logger;


            ConnectionVM = connection;

            MinimizeCommand = new RelayCommand(MinimizeWindow);
            MaximizeCommand = new RelayCommand(MaximizeWindow);
            CloseCommand = new RelayCommand(CloseWindow);

            ShowFirstScreenCommand = new NavigationCommand<FirstScreenViewModel>(_navigation);

            ShowSecondScreenCommand = new NavigationCommand<SecondScreenViewModel>(_navigation);

            ShowInicializeScreenCommand = new NavigationCommand<InicializeScreenViewModel>(_navigation);

            ShowconfigurationScreenCommand = new NavigationCommand<ConfigurationScreenViewModel>(_navigation);
            
            ShowConfigScreenCommand = new NavigationCommand<ConfigurationScreenEditViewModel>(_navigation);


            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            Inicialize();
        }

       

        private async void Inicialize()
        {
            _logger.Info(LogCategory.Application, "Inicializando aplicacion");

            bool is_db_conected = await _database.TestConnectionAsync();
            if (is_db_conected)
            {

                _logger.Info(LogCategory.Database,"Conexion correcta con la base de datos");

                _appState.Status = AppStatus.Idle;
            }
            else
            {

                _logger.Info(LogCategory.Database,"Conexion fallida con la base de datos");

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
            _logger.Info(LogCategory.Application,"Cerrando aplicación");

            Application.Current.Shutdown();
        }
    
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentView));
        }

      
    }
}
