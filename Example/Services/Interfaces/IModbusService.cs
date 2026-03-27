using EthModbus.Models.Modbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EthModbus.Services.Interfaces
{
    public interface IModbusService
    {
        void Connect(string host,int port);
        void Disconnect();

        bool IsConnected { get; }

        DiscreteCoil ReadCoil(ushort address);
        
        IReadOnlyList<DiscreteCoil> ReadCoils(IEnumerable<ushort> addresses);  
        
        void WriteCoil(DiscreteCoil coil,bool value);

    }
}
