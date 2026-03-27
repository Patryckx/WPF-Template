using EthModbus.Services.Interfaces;
using Example.Models;
using Example.ViewModels.Base;
using Modbus.Device;
using ModbusTcpLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Example.ViewModels
{
    public class ConnectionViewModel : ViewModelBase
    {

        private CancellationTokenSource _cts;

        private readonly IModbusService _modbus;

        public ICommand ConnectCommand { get; }

        public ICommand DisconnectCommand { get; }

        private ProcessStatus _plcStatus;

        private CoilStatus _coilStatus;

        public ProcessStatus PlcStatus
        {
            get => _plcStatus;
            set
            {
                _plcStatus = value;
                OnPropertyChanged();
            }

        }

        public CoilStatus CoilStatus
        {
            get => _coilStatus;
            set
            {
                _coilStatus = value;
                OnPropertyChanged();
            }
        }

        public ConnectionViewModel(IModbusService modbus)
        {
            _modbus = modbus;
            ConnectCommand = new RelayCommand(Connect);
            DisconnectCommand = new RelayCommand(Disconnect);
        }

        private async void Connect()
        {
            Debug.WriteLine("Iniciando conexión...");

            try
            {
                PlcStatus = ProcessStatus.OnProcess;
                Debug.WriteLine("Estado: Connecting");

                await Task.Run(() => _modbus.Connect("192.168.4.1", 502));

                PlcStatus = ProcessStatus.Sucess;
                Debug.WriteLine("Estado: Connected");

                _cts = new CancellationTokenSource();
                _ = MonitorCoils(_cts.Token);

                Debug.WriteLine("Monitoreo iniciado");
            }
            catch (Exception ex)
            {
                PlcStatus = ProcessStatus.Failed;
                Debug.WriteLine($"Error: {ex.Message}");
                MessageBox.Show("Conexion fallida");
            }
        }

        private void Disconnect()
        {
            _modbus.Disconnect();

            _cts?.Cancel();
            PlcStatus = ProcessStatus.Idle;
        }

        private async Task MonitorCoils(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var coil = await Task.Run(() => _modbus.ReadCoil(0));

                    Debug.WriteLine($"Coil 0:{coil.Value}");

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CoilStatus = coil.Value
                            ? CoilStatus.Enabled
                            : CoilStatus.Disabled;
                    });

                    Debug.WriteLine($"Coil status:{CoilStatus}");

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Coil error: {ex.Message}");
                }

                await Task.Delay(500, token);
            }
        }
    }
}
