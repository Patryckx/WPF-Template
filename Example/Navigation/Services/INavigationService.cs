using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Example.ViewModels.Base;

namespace Example.Navigation.Services
{
    public interface INavigationService
    {

        void Navigate<TViewModel>()
            where TViewModel : ViewModelBase;

    }
}
