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
using LocalStorageLibrary.Interfaces;

namespace Example.Services
{
    public class StartupService :IStartupService
    {
        private readonly IConfigService _config;
        private readonly ILoggerService _logger;
        private readonly IDataService _database;
        private readonly ILocalDatabase _localDatabase;


        public StartupService(
            IConfigService config,
            ILoggerService logger,
            IDataService database,
            ILocalDatabase localDatabase)
        {
            _config = config;
            _logger = logger;
            _database = database;
            _localDatabase = localDatabase;
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
                Message = "Verificando almacenamiento local..."
            });

            await InitializeLocalDatabaseAsync();
            await Task.Delay(1000);

            progress.Report(new StartupProgress
            {
                Progress = 90,
                Message = "Finalizando inicializacion"
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



        //SERVER DATABASE TASK
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


        //LOCAL DATABASE TASK

        private async Task InitializeLocalDatabaseAsync()
        {
            _logger.Info(LogCategory.Database, "Inicializando almacenamiento local (SQLITE)");

            await _localDatabase.InitializeAsync();


            _logger.Info(LogCategory.Database, "Almacenamiento Inicializado");

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