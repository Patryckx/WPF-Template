using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T :class;
    }
}
