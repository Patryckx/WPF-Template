using Example.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Navigation.Services
{
    public interface IViewModelFactory
    {

        TViewModel Create<TViewModel>()
            where TViewModel : ViewModelBase;
    }
}
