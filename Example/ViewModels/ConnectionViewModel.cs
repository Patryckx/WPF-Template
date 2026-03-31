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

        public ObservableCollection<DiscreteCoil> Coils

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

        private const int MAX_RETRIES = 3;
        private const int RETRY_DELAY_MS = 2000;

        private const int MONITOR_INTERVAL_MS = 500;

        private async Task MonitorCoils(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bool readSuccess = await TryReadCoils(token);

                if (!readSuccess)
                {
                    // Lectura falló — intenta reconectar
                    bool reconnected = await TryReconnect(token);

                    if (!reconnected)
                    {
                        // No se pudo reconectar tras los reintentos
                        PlcStatus = ProcessStatus.Failed;
                        Debug.WriteLine("Monitoreo detenido — sin conexión.");
                        return;
                    }
                }

                await Task.Delay(MONITOR_INTERVAL_MS, token);
            }
        }
        private async Task<bool> TryReadCoils(CancellationToken token)
        {
            try
            {
                var results = await Task.Run(
                    () => _modbus.ReadCoilRange(COIL_START_ADDRESS, COIL_COUNT), token);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        Coils[i].Value = results[i].Value;
                        Coils[i].IsValid = results[i].IsValid;
                        Coils[i].LastUpdated = results[i].LastUpdated;
                    }

                    // Restaura el estado si venía de un error
                    if (PlcStatus != ProcessStatus.Sucess)
                        PlcStatus = ProcessStatus.Sucess;
                });

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error de lectura: {ex.Message}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    PlcStatus = ProcessStatus.OnProcess; // Indica reintentando
                    foreach (var coil in Coils)
                        coil.IsValid = false;
                });

                return false;
            }
        }

        private async Task<bool> TryReconnect(CancellationToken token)
        {
            for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
            {
                if (token.IsCancellationRequested)
                    return false;

                Debug.WriteLine($"Reconexión intento {attempt}/{MAX_RETRIES}...");

                try
                {
                    await Task.Run(() =>
                    {
                        _modbus.Disconnect();
                        _modbus.Connect("192.168.4.1", 502);
                    }, token);

                    Debug.WriteLine("Reconexión exitosa.");

                    Application.Current.Dispatcher.Invoke(() =>
                        PlcStatus = ProcessStatus.Sucess);

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Intento {attempt} fallido: {ex.Message}");

                    Application.Current.Dispatcher.Invoke(() =>
                        PlcStatus = ProcessStatus.OnProcess);

                    // Espera progresiva entre intentos: 2s, 4s, 6s
                    await Task.Delay(RETRY_DELAY_MS * attempt, token);
                }
            }

            return false;
        }
    }
}
