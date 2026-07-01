using Example.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Example.ViewModels.Base;
using Example.Navigation.Services;

namespace Example.Navigation.Commands
{
    public class NavigationCommand<TViewModel> :ICommand
        where TViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public NavigationCommand (INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public event EventHandler? CanExecuteChanged;


        public bool CanExecute(object? parameter)
        {
            return true;
        }


        public void Execute(object? parameter)
        {
            _navigationService.Navigate<TViewModel>();
        }
    }
}
