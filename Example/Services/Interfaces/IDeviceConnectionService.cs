using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthModbus.Models;
using EthModbus.Models.Modbus;

namespace EthModbus.Services.Interfaces
{
    public interface IDeviceConnectionService
    {
        DeviceConnection Connect(DeviceConnection device);

        void Disconnect();

        DeviceConnection GetStatus();


    }
}
