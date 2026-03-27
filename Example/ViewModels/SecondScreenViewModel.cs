using EthModbus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    public class SecondScreenViewModel
    {
        public ConnectionViewModel ConnectionVM { get; }

        public SecondScreenViewModel ()
        {
            ConnectionVM=new ConnectionViewModel(new ModbusService());
        }

    }
}
