using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EthModbus.Models.Modbus;
using EthModbus.Services.Interfaces;
using ModbusTcpLib.Interfaces;
using ModbusTcpLib;

namespace EthModbus.Services
{
    public class DeviceConnectionService : IDeviceConnectionService
    {

        private readonly IModbusClient _client;

        private DeviceConnection _currentDevice;

        public DeviceConnectionService(IModbusClient client)
        {
            _client = client;
        }

        public DeviceConnection Connect(DeviceConnection device)
        {
            try
            {
                _client.Connect(device.IpAddress, device.Port);
                device.IsConnected = true;
                device.LastConnectionAttempt = DateTime.Now;

                _currentDevice = device;


            }
            catch (Exception)

            {
                device.IsConnected = false;
                device.LastConnectionAttempt = DateTime.Now;
                throw;
            }
            return device;
        }

        public void Disconnect()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }

            if (_currentDevice != null)
            {
                _currentDevice.IsConnected = false;
            }

        }

        public DeviceConnection GetStatus()
        {
            return _currentDevice;
        }
    }
}
