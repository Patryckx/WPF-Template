using ConfigIniLib.interfaces;
using Example.Models;
using Example.Navigation.Services;
using Example.Services.Interfaces;
using Example.ViewModels.Base;


using IndustrialSerialTool.Devices;
using IndustrialSerialTool.Interfaces;
using IndustrialSerialTool.Drivers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Example.ViewModels
{
    public class DCVoltage_test_ViewModel :ViewModelBase
    {

        private readonly IConfigService _config;
        private readonly ILoggerService _logger;
        private readonly ISerialDevice _serialDevice;
        private readonly OwonXdm1041Driver _multimeter;

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

        public DCVoltage_test_ViewModel(IConfigService config ,ILoggerService looger,ISerialDevice serialDevice)
        {
            _config = config;
            _logger = looger;
            _serialDevice = serialDevice;

            _multimeter = new OwonXdm1041Driver(_serialDevice);

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

            try
            {

                if(!_serialDevice.IsConnected)
                {
                    bool connected = _serialDevice.Connect();

                    if (!connected)
                    {
                        _logger.Error(LogCategory.Application, "No se pudo conectar al puerto serial");
                        return;
                    }
                }

                string? rawResponse = await _multimeter.GetVdcAsync();

                if (string.IsNullOrWhiteSpace(rawResponse))
                {
                    _logger.Warn(LogCategory.Application, "El multimetro no devolvio respuesta");
                    return;
                }
                _logger.Info(LogCategory.Application, $"Lectura raw recibida {rawResponse}");

                //Convertir respuesta a double
                // Usamos CultureInfo.InvariantCulture porque SCPI responde en formato inglés (ej: "12.045" o "1.204E+01")
                if (double.TryParse(rawResponse, NumberStyles.Float, CultureInfo.InvariantCulture, out double measuredVoltage))

                {
                    _logger.Info(LogCategory.Application, $"Voltaje DC medido: {measuredVoltage}");


                    //Verificar evaluacion con valores limites 

                    bool isPass = measuredVoltage >= DC_voltage_test_lower_limit && measuredVoltage <= DC_voltage_test_upper_limit;

                    if (isPass)

                    {
                        _logger.Info(LogCategory.Application, $"Resultado :PASS (medicion dentro del rango permitido");
                    }
                    else
                    {
                        _logger.Warn(LogCategory.Application, $"Resultado :FAIL (medicion fuera del rango permitido [{DC_voltage_test_lower_limit} V - {DC_voltage_test_upper_limit} V]).");

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
