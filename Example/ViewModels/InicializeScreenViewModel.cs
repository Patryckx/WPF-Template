using Example.Navigation.Services;
using Example.ViewModels.Base;

namespace Example.ViewModels
{
    public class InicializeScreenViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public RelayCommand StartCommand { get; }


        public InicializeScreenViewModel(
            INavigationService navigationService)
        {
            _navigationService = navigationService;

            StartCommand = new RelayCommand(Start);
        }


        private void Start()
        {
            _navigationService.Navigate<InnitScreenViewModel>();
        }
    }
}