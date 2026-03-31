using EthModbus.Models.Modbus;
using EthModbus.Services.Interfaces;
using Example.Models;
using Example.ViewModels.Base;
using Modbus.Device;
using ModbusTcpLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;


namespace Example.ViewModels
{
    public class ConnectionViewModel : ViewModelBase
    {
        //Direcciones fijas bobinas

        private const ushort COIL_START_ADDRESS = 0;

        private const ushort COIL_COUNT = 4;

        private CancellationTokenSource _cts;

        private readonly IModbusService _modbus;

        public ICommand ConnectCommand { get; }

        public ICommand DisconnectCommand { get; }


        private CoilStatus _coilStatus;

        private ObservableCollection<DiscreteCoil> _coils;

        private ObservableCollection<DiscreteCoil> Coils

        {
            get => _coils;
            set { _coils = value; OnPropertyChanged(); }

        }


        private ProcessStatus _plcStatus;

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

            Coils = new ObservableCollection<DiscreteCoil>(
                    Enumerable.Range(0, COIL_COUNT).Select(i=> new DiscreteCoil

                    {
                        Address=(ushort)i,
                        Value=false,
                        IsValid=false

                    })
                
                );


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
                    //var coil = await Task.Run(() => _modbus.ReadCoil(0), token);
                    var results = await Task.Run(() => _modbus.ReadCoilRange(COIL_START_ADDRESS, COIL_COUNT), token);



                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        for (int i = 0; i < results.Count; i++)
                        {
                            Coils[i].Value = results[i].Value;
                            Coils[i].IsValid=results[i].IsValid;
                            Coils[i].LastUpdated=results[i].LastUpdated;

                        }
                    });
                }
                catch (OperationCanceledException)
                {
                    break; // salida limpia
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Coil error: {ex.Message}");

                    // ⚠️ En lugar de romper el loop, espera y reintenta
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CoilStatus = CoilStatus.Idle; // indica pérdida temporal
                    });

                    await Task.Delay(2000, token); // espera más antes de reintentar
                    continue;
                }

                await Task.Delay(500, token);
            }
        }
    }
}
