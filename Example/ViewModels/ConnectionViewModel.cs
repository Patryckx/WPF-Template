using ConfigIniLib.interfaces;
using EthModbus.Models.Modbus;
using EthModbus.Services.Interfaces;
using Example.Models;
using Example.Models.Database_registers;
using Example.Services.Interfaces;
using Example.ViewModels.Base;

using Modbus.Device;
using ModbusTcpLib;
using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
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



        //Database register service

        private readonly IRegisterService _registerService;


        private readonly IAppStateService _appState;


        private readonly IDialogService _dialogService;

        private readonly ILoggerService _loggerService;

        public ConnectionViewModel(
            IModbusService modbus,
            IConfigService config,
            IRegisterService registerService,
            IAppStateService appState,
            IDialogService dialogService,
            ILoggerService loogerService
            )
        {
            _modbus = modbus;
            _config= config;
            _registerService = registerService;
            _appState = appState;
            _dialogService = dialogService;
            _loggerService = loogerService;



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

                _loggerService.Info(LogCategory.Modbus, "Iniciando conexion modbus");

                try
                {
                    PlcStatus = ProcessStatus.OnProcess;

                    //await Task.Run(() => _modbus.Connect("192.168.4.1", 502));
                    await Task.Run(() => _modbus.Connect(_config.DAQHost,_config.Port));

                    PlcStatus = ProcessStatus.Sucess;
                    _loggerService.Info(LogCategory.Modbus, $"Conexion exitosa a {_config.DAQHost}:{_config.Port}");

                    _cts = new CancellationTokenSource();
                    _ = MonitorCoils(_cts.Token);

                    _connection_inicialized = true;

                    ConnectCommand.RaiseCanExecuteChanged();
                    DisconnectCommand.RaiseCanExecuteChanged();
                    ToggleCoilCommand.RaiseCanExecuteChanged();


                    _appState.Status = AppStatus.AllDevicesConnected;

                }
                catch (Exception ex)
                {
                    _appState.Status = AppStatus.Error;
                    
                    PlcStatus = ProcessStatus.Failed;

                    _loggerService.Error(
                        LogCategory.Modbus,
                        $"Timeout while connecting to {_config.DAQHost}:{_config.Port}",
                        ex);

                    _dialogService.ShowError("Conexion fallida");

                }

            }
            else
            {
                _loggerService.Info(LogCategory.Modbus, $"Conexion ya inicializada previamente " +
                    $"{_config.DAQHost}:{_config.Port}");

                _dialogService.ShowMessage("Conexion ya inicializada...");
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
        private async void ToggleCoil(DiscreteCoil coil)
        {
            if (coil == null) return;

            bool newValue = !coil.Value;
            _modbus.WriteCoil(coil.Address, newValue);

            coil.Value = newValue;


            await _registerService.AddLogAsync(
                new Coil_register
                {
                    IPAddress = _config.DAQHost,

                    Port = _config.Port.ToString(),


                    Action = $"Coil {coil.Address}" + (newValue ? "ON" : "OFF"),

                    Date = DateTime.Now
                });
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

                        _loggerService.Error(LogCategory.Modbus, "Conexion Modbus interrumpida,monitoreo detenido");

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
                _loggerService.Info(LogCategory.Modbus, "Conexion cancelada,monitoreo detenido");

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

                _loggerService.Info(LogCategory.Modbus, $"Error de lectura: {ex.GetType().Name} - {ex.Message}");

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

                _loggerService.Info(LogCategory.Modbus, $"Reconexión intento {attempt}/{MAX_RETRIES}...");

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
                        _modbus.Connect(_config.DAQHost, _config.Port);

                    }, token);

                    _loggerService.Info(LogCategory.Modbus, $"Reconexión exitosa en intento {attempt}.");


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

                    _loggerService.Info(LogCategory.Modbus, $"Intento {attempt}/{MAX_RETRIES} fallido:");
                    _loggerService.Info(LogCategory.Modbus, $"  Tipo:    {ex.GetType().Name}");
                    _loggerService.Info(LogCategory.Modbus, $"  Mensaje: {ex.Message}");
                    _loggerService.Info(LogCategory.Modbus, $"  Inner:   {ex.InnerException?.Message}");

                    Application.Current.Dispatcher.Invoke(() =>
                        PlcStatus = ProcessStatus.OnProcess);

                    await Task.Delay(RETRY_DELAY_MS * attempt, token);
                }
            }


            _loggerService.Info(LogCategory.Modbus, "Reconexión fallida tras todos los intentos.");

            return false;
        }



       
    }
}
