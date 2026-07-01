using Example.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Navigation.Services
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _provider;

        public ViewModelFactory(IServiceProvider provider)
        {
            _provider = provider;
        }


        public TViewModel Create<TViewModel>()
            where TViewModel:ViewModelBase
        {
            return _provider.GetRequiredService<TViewModel>();  
        }


    }
}
