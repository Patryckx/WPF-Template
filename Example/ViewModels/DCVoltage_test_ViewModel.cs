using ConfigIniLib.interfaces;
using Example.Models;
using Example.Navigation.Services;
using Example.Services.Interfaces;
using Example.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    public class DCVoltage_test_ViewModel :ViewModelBase
    {

        private readonly IConfigService _config;
        private readonly ILoggerService _logger;

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

        public DCVoltage_test_ViewModel(IConfigService config ,ILoggerService looger)
        {
            _config = config;
            _logger = looger;

            load_test_configuration();

        }

        private void load_test_configuration()
        {
            DC_voltage_test_upper_limit = _config.dc_voltage_test_upper_limit;
            DC_voltage_test_lower_limit = _config.dc_voltage_test_lower_limit;


            Console.WriteLine(DC_voltage_test_upper_limit);

            Console.WriteLine(DC_voltage_test_lower_limit);

            _logger.Info(LogCategory.Application, $"DC Voltage test");


            _logger.Info(LogCategory.Application, $"Upper limit: {DC_voltage_test_upper_limit}");

            _logger.Info(LogCategory.Application, $"Lower limit: {DC_voltage_test_lower_limit}");

        }


    }
}
