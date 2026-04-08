using ConfigIniLib;
using ConfigIniLib.interfaces;
using EthModbus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Example.ViewModels
{
    public class SecondScreenViewModel
    {
        public ConnectionViewModel ConnectionVM { get; }

        public SecondScreenViewModel ()
        {

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            IConfigService config = new AppConfigService(configPath);
            ConnectionVM =new ConnectionViewModel(new ModbusService(),config);
        }

    }
}
