using ConfigIniLib;
using ConfigIniLib.interfaces;
using EthModbus.Services.Interfaces;
using Example;
using Example.Models;
using Example.Navigation.Services;
using Example.Navigation.Stores;
using Example.Services;
using Example.Services.Database;
using Example.Services.Interfaces;
using Example.Services.Modbus;
using Example.ViewModels;
using IndustrialSerialTool.Devices;
using IndustrialSerialTool.Interfaces;
using IndustrialSerialTool.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ModbusTcpLib;
using SqlUtilityLibrary.Interfaces;
using SqlUtilityLibrary.Models;
using SqlUtilityLibrary.Services;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using IOPath = System.IO.Path;

namespace Example; //IMPORTANTE VERIFICAR QUE ESTE ARCHIVO TENGA EL ESPACIO DE NOMBRES DE LO CONTRARIO NO SE EJECUTARA LA CLASE APP DEBIDO,

public partial class App : Application
{
    //public App()
    //{
    //    MessageBox.Show("Constructor App");  IMPORTANTE AL DEFINIR UN CONSTRUCTOR EN APP.XAML.CS ES NECESARIO INDICAR QUE SE EJECUTE 
   //                                           inicializeComponent ya que al no tener constructor el compilador genera uno en automatico
    //}
    private ServiceProvider _services;

    private ILoggerService _logger;

    protected override void OnStartup(StartupEventArgs e)
    {

        //******* LOGGER SERVICE-> Subscribe to not controlled exceptions for log service **********

        DispatcherUnhandledException += OnDispatcherUnhandledException;

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        //***************************************************************************


        base.OnStartup(e);

        var sb = new StringBuilder();

        try
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            _services = services.BuildServiceProvider();

            _logger = _services.GetRequiredService < ILoggerService >();

            var navigation = _services.GetRequiredService<INavigationService>();

            navigation.Navigate<InicializeScreenViewModel>();
            //navigation.Navigate<FirstScreenViewModel>();

            var window = _services.GetRequiredService<MainWindow>();

            window.Show();
        }
        catch (Exception ex)
        {
            Exception? current = ex;

            while (current != null)
            {
                MessageBox.Show(
                    $"TIPO:\n{current.GetType().FullName}\n\n" +
                    $"MENSAJE:\n{current.Message}\n\n" +
                    $"STACK:\n{current.StackTrace}");

                current = current.InnerException;
            }
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {

        services.AddSingleton<IStartupService, StartupService>();

        services.AddSingleton<IAppStateService, AppStateService>();

        services.AddSingleton<MainWindow>();

        services.AddSingleton<IDataService>(provider =>

        {
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("AppSettings.json")
            .Build();


            string? connection = configuration["Database_settings:ConnectionString"];


            DatabaseConfig db = new()
            {
                ConnectionString = connection
            };

            return new SqlDatabaseService(db);

        });

        services.AddSingleton<IRegisterService, RegisterService>();

        services.AddSingleton<IConfigService>(provider =>
        {
            string path =
            IOPath.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "config.ini");


            return new AppConfigService(path);
        });

        services.AddSingleton<IModbusService,ModbusService>();

        services.AddSingleton<NavigationStore>();

        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();

        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<ILoggerService, LoggerService>();

        services.AddSingleton<ISerialDevice>(provider =>
        {
            // 1. Obtenemos el IConfigService para leer la configuración del puerto (si existe allí)
            var config = provider.GetRequiredService<IConfigService>();

            // 2. Creamos los settings (puedes mapear las propiedades de tu config.ini o usar valores por defecto)
            var settings = new SerialSettings
            {
                PortName = config.DMM_port ?? "COM5", // Reemplaza por la propiedad real de tu config.ini
                baudRate = config.DMM_Bauds > 0 ? config.DMM_Bauds : 115200,
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };

            // 3. Retornamos la instancia de SerialDevice asignada a la interfaz ISerialDevice
            return new SerialDevice(settings);
        });

        //ConnectionViewModels

        services.AddSingleton<ConnectionViewModel>();

        services.AddSingleton<MainViewModel>();

        //services.AddSingleton<ConfigurationScreenViewModel>();
        services.AddTransient<ConfigurationScreenViewModel>();

        //services.AddSingleton<ConfigurationScreenEditViewModel>();
        services.AddTransient<ConfigurationScreenEditViewModel>();

        //Views navigation service

        services.AddTransient<FirstScreenViewModel>();

        services.AddTransient<SecondScreenViewModel>();

        services.AddTransient<InicializeScreenViewModel>();

        services.AddTransient<InnitScreenViewModel>();

        services.AddTransient<DCVoltage_test_ViewModel>();
    }

    //************************  LOG CATCH FUNCTIONS ******************************
    private void OnDispatcherUnhandledException(
        object senser,DispatcherUnhandledExceptionEventArgs e )
    {
        _logger.Error(LogCategory.System,
            "Unhandled UI Exception",
            e.Exception);

        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender,
        UnhandledExceptionEventArgs e)
    {

        if(e.ExceptionObject is Exception ex)
        {
            _logger.Error(
                LogCategory.System,
                "Fatal application error",
                ex);
        }

    }

    private void TaskScheduler_UnobservedTaskException(
        object? sender,
        UnobservedTaskExceptionEventArgs e)
    {
        _logger.Error(LogCategory.System,
            "Task exception",
            e.Exception);

        e.SetObserved();
    }









}