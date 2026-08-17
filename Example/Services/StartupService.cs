using ConfigIniLib.interfaces;
using Example.Models;
using Example.Services.Interfaces;
using SqlUtilityLibrary.Interfaces;
using SqlUtilityLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services
{
    public class StartupService :IStartupService
    {
        private readonly IConfigService _config;
        private readonly ILoggerService _logger;
        private readonly IDataService _database;

        public StartupService(
            IConfigService config,
            ILoggerService logger,
            IDataService database)
        {
            _config = config;
            _logger = logger;
            _database = database;
        }

        public async Task InitializeAsync(
            IProgress<StartupProgress> progress)
        {
            progress.Report(new StartupProgress
            {
                Progress = 0,
                Message = "Iniciando aplicación..."
            });

            await Task.Delay(1000);

            progress.Report(new StartupProgress
            {
                Progress = 20,
                Message = "Leyendo configuración..."
            });

            await InitializeConfigurationAsync();
            await Task.Delay(1000);

            progress.Report(new StartupProgress
            {
                Progress = 40,
                Message = "Inicializando servicios..."
            });

            await InitializeServicesAsync();
            await Task.Delay(1000);

            progress.Report(new StartupProgress
            {
                Progress = 60,
                Message = "Probando conexión con base de datos..."
            });

            await TestDatabaseAsync();
            await Task.Delay(1000);

            progress.Report(new StartupProgress
            {
                Progress = 80,
                Message = "Finalizando inicialización..."
            });

            await FinalizeStartupAsync();
            await Task.Delay(1000);

            progress.Report(new StartupProgress
            {
                Progress = 100,
                Message = "Inicialización completada."
            });
        }


        //Process

        private Task InitializeServicesAsync()
        {
            return Task.CompletedTask;
        }

        private Task InitializeConfigurationAsync()
        {

            _logger.Info(
        LogCategory.Configuracion,
        "Verificando configuración de la aplicación.");

            if (string.IsNullOrWhiteSpace(_config.DAQHost))
            {
                _logger.Warn(
                    LogCategory.Configuracion,
                    "DAQHost no está configurado.");
            }

            if (string.IsNullOrWhiteSpace(_config.DatabaseAddress))
            {
                _logger.Warn(
                    LogCategory.Configuracion,
                    "DatabaseAddress no está configurado.");
            }

            if (string.IsNullOrWhiteSpace(_config.DMM_port))
            {
                _logger.Warn(
                    LogCategory.Configuracion,
                    "Puerto del multímetro no está configurado.");
            }

            if (string.IsNullOrWhiteSpace(_config.Camera_address))
            {
                _logger.Warn(
                    LogCategory.Configuracion,
                    "Dirección de cámara no está configurada.");
            }

            _logger.Info(
                LogCategory.Configuracion,
                "Verificación de configuración finalizada.");

            return Task.CompletedTask;
        }




        private async Task TestDatabaseAsync()
        {
            _logger.Info(
           LogCategory.Database,
           "Probando conexión con la base de datos.");

                bool connected =
                    await _database.TestConnectionAsync();

                if (connected)
                {
                    _logger.Info(
                        LogCategory.Database,
                        "Conexión con la base de datos exitosa.");
                }
                else
                {
                    _logger.Warn(
                        LogCategory.Database,
                        "No fue posible conectar con la base de datos.");
                }
        }






        private Task FinalizeStartupAsync()
        {
            _logger.Info(
                LogCategory.System,
                "Inicialización de la aplicación finalizada.");

            return Task.CompletedTask;
        }
    }
}