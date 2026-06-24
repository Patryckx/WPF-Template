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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    public class SecondScreenViewModel
    {
        public ConnectionViewModel ConnectionVM { get; }

        private readonly IDataService _database;


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
        }

    }
}
