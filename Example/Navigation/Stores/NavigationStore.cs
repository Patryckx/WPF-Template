using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.ViewModels.Base;


namespace Example.Navigation.Stores
{
    public class NavigationStore
    {
        private ViewModelBase? _currentViewModel;

        public ViewModelBase? CurrentViewModel
        {
            get => _currentViewModel;

            set
            {

                if (_currentViewModel == value)
                    return;
 
                _currentViewModel = value;

                OnCurrentViewModelChanged();
            }
        }

        public event Action? CurrentViewModelChanged;
        private void OnCurrentViewModelChanged()
        {
           CurrentViewModelChanged?.Invoke();
        }

    }
}


