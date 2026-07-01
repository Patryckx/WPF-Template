using Example.Navigation.Stores;
using Example.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Navigation.Services
{
    public class NavigationService : INavigationService
    {

        private readonly IViewModelFactory _factory;

        private readonly NavigationStore _store;


        public NavigationService(
            IViewModelFactory factory,
            NavigationStore store)
        {
            _factory = factory;
            _store = store;
        }


        public void Navigate<T>()
            where T :ViewModelBase
        {
            _store.CurrentViewModel =
                _factory.Create<T>();
        }


    }
}
