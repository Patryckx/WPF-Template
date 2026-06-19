using ConfigIniLib.interfaces;
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
        //Configuration
        private readonly IConfigService _config;

        //Banderas

        private bool _connection_inicialized;

        //Direcciones fijas bobinas

        private const ushort COIL_START_ADDRESS = 0;

        private const ushort COIL_COUNT = 4;

        private CancellationTokenSource _cts;

        private readonly IModbusService _modbus;

        public RelayCommand ConnectCommand { get; }

        public RelayCommand DisconnectCommand { get; }

        public RelayCommand<DiscreteCoil> ToggleCoilCommand {  get; }


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

        public ConnectionViewModel(IModbusService modbus,IConfigService config)
        {
            _modbus = modbus;
            _config= config;

            ConnectCommand = new RelayCommand(
                execute: Connect,
                canExecute:()=> !_modbus.IsConnected
                );
            DisconnectCommand = new RelayCommand(
                
                execute:Disconnect,
                canExecute:()=>_modbus.IsConnected
                
                );

            ToggleCoilCommand = new RelayCommand<DiscreteCoil>(
                
                execute:ToggleCoil,
                canExecute:coil=>_modbus.IsConnected && coil!=null
                
                );

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
            if (_connection_inicialized == false) {


                Debug.WriteLine("Iniciando conexión...");

                try
                {
                    PlcStatus = ProcessStatus.OnProcess;
                    Debug.WriteLine("Estado: Connecting");

                    //await Task.Run(() => _modbus.Connect("192.168.4.1", 502));
                    await Task.Run(() => _modbus.Connect(_config.Host,_config.Port));

                    PlcStatus = ProcessStatus.Sucess;
                    Debug.WriteLine("Estado: Connected");

                    _cts = new CancellationTokenSource();
                    _ = MonitorCoils(_cts.Token);

                    Debug.WriteLine("Monitoreo iniciado");

                    _connection_inicialized = true;

                    ConnectCommand.RaiseCanExecuteChanged();
                    DisconnectCommand.RaiseCanExecuteChanged();
                    ToggleCoilCommand.RaiseCanExecuteChanged();

                }
                catch (Exception ex)
                {
                    PlcStatus = ProcessStatus.Failed;
                    Debug.WriteLine($"Error: {ex.Message}");
                    MessageBox.Show("Conexion fallida");
                }

            }
            else
            {
                Debug.WriteLine("Connection already inicializaed");
            }
        }

        private void Disconnect()
        {
            _modbus.Disconnect();

            _cts?.Cancel();
            PlcStatus = ProcessStatus.Idle;

            _connection_inicialized = false;
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
            ToggleCoilCommand.RaiseCanExecuteChanged();
        }


        private void ToggleCoil(DiscreteCoil coil)
        {
            if (coil == null) return;

            bool newValue = !coil.Value;
            _modbus.WriteCoil(coil.Address, newValue);

            coil.Value = newValue;
            
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

                //await Task.Delay(MONITOR_INTERVAL_MS, token);
                await Task.Delay(_config.MonitorIntervalMs, token);
            }
        }
        private async Task<bool> TryReadCoils(CancellationToken token)
        {
            //  Verifica el estado del socket antes de intentar leer
            if (!_modbus.IsConnected)
            {
                Debug.WriteLine("Cliente desconectado detectado antes de leer.");
                return false;
            }

            try
            {
                //var results = await Task.Run(
                //    () => _modbus.ReadCoilRange(COIL_START_ADDRESS, COIL_COUNT), token);
                var results = await Task.Run(
                    () => _modbus.ReadCoilRange(_config.CoilStartAddress, _config.CoilCount), token);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        Coils[i].Value = results[i].Value;
                        Coils[i].IsValid = results[i].IsValid;
                        Coils[i].LastUpdated = results[i].LastUpdated;
                    }

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
                Debug.WriteLine($"Error de lectura: {ex.GetType().Name} - {ex.Message}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    PlcStatus = ProcessStatus.OnProcess;
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
                        // Siempre desconecta primero, ignorando cualquier error
                        // Esto garantiza que _tcpClient quede en null
                        try { _modbus.Disconnect(); } catch { }

                        // Da tiempo al ESP32 para liberar el socket
                        // y al TcpClient para cerrarse completamente
                        Thread.Sleep(1500);

                        // Ahora Connect() no encontrará IsConnected = true
                        _modbus.Connect(_config.Host, 502);

                    }, token);

                    Debug.WriteLine($"Reconexión exitosa en intento {attempt}.");

                    Application.Current.Dispatcher.Invoke(() =>
                        PlcStatus = ProcessStatus.Sucess);

                    return true;
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Intento {attempt}/{MAX_RETRIES} fallido:");
                    Debug.WriteLine($"  Tipo:    {ex.GetType().Name}");
                    Debug.WriteLine($"  Mensaje: {ex.Message}");
                    Debug.WriteLine($"  Inner:   {ex.InnerException?.Message}");

                    Application.Current.Dispatcher.Invoke(() =>
                        PlcStatus = ProcessStatus.OnProcess);

                    await Task.Delay(RETRY_DELAY_MS * attempt, token);
                }
            }

            Debug.WriteLine("Reconexión fallida tras todos los intentos.");
            return false;
        }



       
    }
}
