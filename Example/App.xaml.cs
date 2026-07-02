using ConfigIniLib;
using ConfigIniLib.interfaces;
using EthModbus.Services;
using EthModbus.Services.Interfaces;
using Example;
using Example.Models;
using Example.Navigation.Services;
using Example.Navigation.Stores;
using Example.Services;
using Example.Services.Interfaces;
using Example.ViewModels;
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
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sb = new StringBuilder();

      

        try
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            _services = services.BuildServiceProvider();

            var navigation = _services.GetRequiredService<INavigationService>();

            navigation.Navigate<FirstScreenViewModel>();

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

        //ConnectionViewModels

        services.AddSingleton<ConnectionViewModel>();

        services.AddSingleton<MainViewModel>();

        services.AddSingleton<ConfigurationScreenViewModel>();

        services.AddSingleton<ConfigurationScreenEditViewModel>();

        //Views navigation service

        services.AddTransient<FirstScreenViewModel>();

        services.AddTransient<SecondScreenViewModel>();

        services.AddTransient<InicializeScreenViewModel>();



    }
}