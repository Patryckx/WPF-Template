using EthModbus.Models.Modbus;
using EthModbus.Services.Interfaces;
using ModbusTcpLib;
using ModbusTcpLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthModbus.Services
{
    public class ModbusService : IModbusService
    {
        private readonly IModbusClient _client;
        private readonly byte _slaveId = 1;

        public ModbusService() {

            _client = new ModbusClient();

        }


        public void Connect(string host,int port)
        {
            _client.Connect(host,port);
        }

        public bool IsConnected => _client.IsConnected;
       

        public void Disconnect()
        {
            _client.Disconnect();
        }



        public DiscreteCoil ReadCoil(ushort address)
        {
            var coil = new DiscreteCoil
            {
                Address = address,
                LastUpdated = DateTime.Now
            };

            try
            {
                coil.Value = _client.ReadSingleCoil(_slaveId, address);
                coil.IsValid = true;
                coil.Error = null;
            }
            catch (Exception ex)
            {
                coil.IsValid= false;
                coil.Error=ex.Message;
            }

            return coil;


        }


        public IReadOnlyList<DiscreteCoil> ReadCoils(IEnumerable<ushort> addresses)
        {
            var coils= new List<DiscreteCoil>();

            foreach (var address in addresses)
            {
                coils.Add(ReadCoil(address));
            }
            return coils;

        }

        public void WriteCoil(DiscreteCoil coil,bool value)
        {
            if (!coil.IsWritable)
                throw new InvalidOperationException("Coil is read-only");

            WriteCoil(coil.Address, value);

            coil.Value = value;
            coil.IsValid = true;
            coil.Error = null;
            coil.LastUpdated = DateTime.UtcNow;
            }


        public void WriteCoil(ushort address, bool value)
        {
            try
            {
                _client.WriteSingleCoil(_slaveId, address, value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing coil {address}: {ex.Message}", ex);
            }
        }




        public IReadOnlyList<DiscreteCoil> ReadCoilRange(ushort startAddress, ushort count)
        {
            var coils = new List<DiscreteCoil>();
            var now = DateTime.Now;

            try
            {
                // Lee todas las bobinas en una sola transacción Modbus
                bool[] values = _client.ReadMultipleCoils(_slaveId, startAddress, count);

                for (ushort i = 0; i < values.Length; i++)
                {
                    coils.Add(new DiscreteCoil
                    {
                        Address = (ushort)(startAddress + i),
                        Value = values[i],
                        IsValid = true,
                        LastUpdated = now
                    });
                }
            }
            catch (Exception ex)
            {
                // Si falla la lectura en bloque, todas quedan inválidas
                for (ushort i = 0; i < count; i++)
                {
                    coils.Add(new DiscreteCoil
                    {
                        Address = (ushort)(startAddress + i),
                        IsValid = false,
                        Error = ex.Message,
                        LastUpdated = now
                    });
                }
            }

            return coils;
        }




    }
}