using ConfigIniLib.interfaces;
using Example.Models;
using Example.Navigation.Services;
using Example.Services;
using Example.Services.Interfaces;
using Example.ViewModels.Base;
using IndustrialSerialTool.Devices;
using IndustrialSerialTool.Drivers;
using IndustrialSerialTool.Interfaces;
using LocalStorageLibrary.Interfaces;
using LocalStorageLibrary.Models;
using LocalStorageLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    public class DCVoltage_test_ViewModel :ViewModelBase
    {

        private readonly IAppStateService _appStateService;
        private readonly IConfigService _config;
        private readonly ILoggerService _logger;
        private readonly ISerialDevice _serialDevice;
        private readonly OwonXdm1041Driver _multimeter;

        private readonly IProductionRepository _productionRepository;

        private DCVoltageTestState _currentTestState =
            DCVoltageTestState.Waiting;

        public DCVoltageTestState CurrentTestState
        {
            get => _currentTestState;
            set
            {
                _currentTestState = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand DC_Voltage_Meassure_Command { get; }


        private double _DC_voltage_test_upper_limit;
        public double DC_voltage_test_upper_limit
        {
            get => _DC_voltage_test_upper_limit;
            set
            {
                _DC_voltage_test_upper_limit = value;
                OnPropertyChanged();
            }
        }

        private double _DC_voltage_test_lower_limit;
        public double DC_voltage_test_lower_limit
        {
            get => _DC_voltage_test_lower_limit;
            set
            {
                _DC_voltage_test_lower_limit = value;
                OnPropertyChanged();
            }
        }

        private string _lastDCVoltageMeassure;

        public string LastDCVoltageMeassure
        {
            get => _lastDCVoltageMeassure;

            set
            {
                _lastDCVoltageMeassure = value;
                OnPropertyChanged();
            }
        }


        private ProcessStatus _testStatus;

        public ProcessStatus testStatus
        {
            get => _testStatus;
            set
            {
                _testStatus = value;
                OnPropertyChanged();
            }
        }


        public DCVoltage_test_ViewModel(
            IAppStateService appStateService,
            IConfigService config ,
            ILoggerService looger,
            ISerialDevice serialDevice,
            IProductionRepository productionRepository
            )
        {
            _appStateService = appStateService;
            _config = config;
            _logger = looger;
            _serialDevice = serialDevice;

            _multimeter = new OwonXdm1041Driver(_serialDevice);

            _productionRepository = productionRepository;

            //comandos asíncronos en MVVM, considera un AsyncRelayCommand
            //DC_Voltage_Meassure_Command = new RelayCommand(DC_Voltage_Meassure);
            DC_Voltage_Meassure_Command = new RelayCommand(async () => await DC_Voltage_MeassureAsync());

            load_test_configuration();

        }

        private void load_test_configuration()
        {
            DC_voltage_test_upper_limit = _config.dc_voltage_test_upper_limit;
            DC_voltage_test_lower_limit = _config.dc_voltage_test_lower_limit;

            Console.WriteLine(DC_voltage_test_upper_limit);

            Console.WriteLine(DC_voltage_test_lower_limit);

            _logger.Info(LogCategory.Application, $"DC Voltage test configuration loaded");

            _logger.Info(LogCategory.Application, $"Upper limit: {DC_voltage_test_upper_limit}");

            _logger.Info(LogCategory.Application, $"Lower limit: {DC_voltage_test_lower_limit}");
        }



        private async Task DC_Voltage_MeassureAsync()
        {

            _logger.Info(LogCategory.Application, "Performing DMM DC voltage test ");

            CurrentTestState = DCVoltageTestState.Waiting;


            testStatus = ProcessStatus.Idle;

            await Task.Delay(1500);

            try
            {

                if(!_serialDevice.IsConnected)
                {
                    bool connected = _serialDevice.Connect();

                    if (!connected)
                    {
                        _logger.Error(LogCategory.Application, "No se pudo conectar al puerto serial");


                        _appStateService.DCVoltageStatus = ProcessStatus.Failed;

                        return;
                    }
                }

                string? rawResponse = await _multimeter.GetVdcAsync();

                if (string.IsNullOrWhiteSpace(rawResponse))
                {
                    _logger.Warn(LogCategory.Application, "El multimetro no devolvio respuesta");

                    LastDCVoltageMeassure = "No se obtuvo una respuesta valida";


                    return;
                }
                _logger.Info(LogCategory.Application, $"Lectura raw recibida {rawResponse}");

                //Convertir respuesta a double
                // Usamos CultureInfo.InvariantCulture porque SCPI responde en formato inglés (ej: "12.045" o "1.204E+01")
                if (double.TryParse(rawResponse, NumberStyles.Float, CultureInfo.InvariantCulture, out double measuredVoltage))

                {
                    _logger.Info(LogCategory.Application, $"Voltaje DC medido: {measuredVoltage}");

                    LastDCVoltageMeassure = measuredVoltage.ToString();

                    //Verificar evaluacion con valores limites 

                    bool isPass = measuredVoltage >= DC_voltage_test_lower_limit && measuredVoltage <= DC_voltage_test_upper_limit;

                    if (isPass)

                    {
                        _logger.Info(LogCategory.Application, $"Resultado :PASS (medicion dentro del rango permitido");

                        CurrentTestState = DCVoltageTestState.Success;

                        testStatus =ProcessStatus.Sucess;

                        await Task.Delay(4000);

                        CurrentTestState = DCVoltageTestState.Waiting;

                        _appStateService.DCVoltageStatus = ProcessStatus.Sucess;

                        var record = new ProductionRecord
                        {
                            TimeStamp = DateTime.Now,
                            Quantity = 1
                        };


                        long id = await _productionRepository.InsertAsync(record);

                    }
                    else
                    {
                        _logger.Warn(LogCategory.Application, $"Resultado :FAIL (medicion fuera del rango permitido [{DC_voltage_test_lower_limit} V - {DC_voltage_test_upper_limit} V]).");
                        CurrentTestState = DCVoltageTestState.Failed;
                        testStatus = ProcessStatus.Failed;
                        await Task.Delay(4000);
                        CurrentTestState = DCVoltageTestState.Waiting;

                        _appStateService.DCVoltageStatus = ProcessStatus.Failed;


                        var record = new ProductionRecord
                        {
                            TimeStamp = DateTime.Now,
                            Quantity = 1
                        };


                        long id = await _productionRepository.InsertAsync(record);

                    }

                }
                else
                {
                    _logger.Warn(LogCategory.Application, $"No se pudo convertir la respuesta '{rawResponse}' a un valor númerico");
                }

            }catch (Exception ex)
            {
                _logger.Error(LogCategory.Application, $"Excepcion en la prueba Voltaje DC : {ex.Message}");
            }
        }
    }
}
