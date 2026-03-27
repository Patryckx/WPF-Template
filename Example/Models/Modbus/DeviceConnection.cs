using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthModbus.Models.Modbus
{
    public class DeviceConnection
    {

        public string Name { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public bool IsConnected {get;set;}
        
        public DateTime LastConnectionAttempt { get; set; }
    }
}
