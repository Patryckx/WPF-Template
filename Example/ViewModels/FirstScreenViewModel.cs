using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Example.ViewModels.Base;

namespace Example.ViewModels
{
    public class FirstScreenViewModel : ViewModelBase
    {

     public ConnectionViewModel ConnectionVM { get;  }

        
     public FirstScreenViewModel(ConnectionViewModel connectionVM)
        {
            ConnectionVM = connectionVM; 
        }
    }
}
