using ConfigIniLib;
using ConfigIniLib.interfaces;
using EthModbus.Services;
using Example.Services;
using Example.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlUtilityLibrary;
using SqlUtilityLibrary.Interfaces;
using SqlUtilityLibrary.Models;
using SqlUtilityLibrary.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.ViewModels.Base;


namespace Example.ViewModels
{
    public class SecondScreenViewModel :ViewModelBase
    {
        public ConnectionViewModel ConnectionVM { get; }

        private readonly IDataService _database;

        private DataView? _coilInfo;

        public DataView? CoilInfo
        {
            get => _coilInfo;
            set
            {
                _coilInfo = value;
                OnPropertyChanged();
            }
        }


        public SecondScreenViewModel ()
        {

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            IConfigService config = new AppConfigService(configPath);

            //Database config
            IConfiguration configuration =
                new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("AppSettings.json")
                .Build();

            string? connectionString = configuration["Database_settings:ConnectionString"];


            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception(
                    "No se encontro la cadena de conexion.");
            }

            DatabaseConfig db_config = new()
            {
                ConnectionString = connectionString,
            };


            _database = new SqlDatabaseService(db_config);

            IRegisterService registerService = new RegisterService(_database);

            ConnectionVM = new ConnectionViewModel(new ModbusService(),config,registerService);
            
            LoadCoilsInfo();
        }


        private async void LoadCoilsInfo()
        {
            DataTable dt =
                await _database.ExecuteQueryAsync(
                    "SELECT * FROM Coils_registers");

            CoilInfo = dt.DefaultView;
        }

    }
}
