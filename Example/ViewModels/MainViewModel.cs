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

        public ViewModelBase? CurrentView
        {
            get => _navigationStore.CurrentViewModel;
        }

        private readonly INavigationService _navigation;
        private readonly IDataService _database;
        private readonly IAppStateService _appState;
        private readonly NavigationStore _navigationStore;


        public IAppStateService AppState => _appState;

        public MainViewModel(
            INavigationService navigation,
            IAppStateService appState, 
            IDataService database,
            ConnectionViewModel connection,
            ConfigurationScreenViewModel configRead,
            ConfigurationScreenEditViewModel configEdit,
            NavigationStore navigationStore
            )
        {

            _navigation = navigation;
            _appState = appState;
            _database = database;
            _navigationStore= navigationStore;
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
      //  public void ShowFirstScreen()
      //  {
      //      //CurrentView = new FirstScreenViewModel(ConnectionVM);

      //      //_navigationStore.CurrentViewModel =
      //      //_serviceProvider.GetRequiredService<FirstScreenViewModel>();

      //      _navigation.Navigate<FirstScreenViewModel>();
      //  }
      //  public void ShowSecondScreen()
      //  {
      //      //CurrentView = new SecondScreenViewModel(ConnectionVM, _database, _appState);

      //       //       _navigationStore.CurrentViewModel =
      //       //_serviceProvider.GetRequiredService<SecondScreenViewModel>();

      //      _navigation.Navigate<SecondScreenViewModel>();


      //  }

      //  public void ShowInicializeScreen()
      //  {
      //    //      _navigationStore.CurrentViewModel =
      //    //_serviceProvider.GetRequiredService<InicializeScreenViewModel>();

      //      _navigation.Navigate<InicializeScreenViewModel> ();
      //  }

      //  public void ShowConfigurationScreen()

      //  {
      //      //CurrentView = new ConfigurationScreenViewModel();
      //      _configReadVM.Refresh();

      ////      _navigationStore.CurrentViewModel =
      ////_serviceProvider.GetRequiredService<ConfigurationScreenViewModel>();
      //  }
      //  private void ShowConfigScreen()
      //  {
      ////      _navigationStore.CurrentViewModel =
      ////_serviceProvider.GetRequiredService<ConfigurationScreenEditViewModel>();
      //  }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentView));
        }

      
    }
}
