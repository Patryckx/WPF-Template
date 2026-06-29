using Example;
using Example.Services;
using Example.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

public partial class App : Application
{
    private ServiceProvider _services;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        ConfigureServices(services);

        _services = services.BuildServiceProvider();

        MainWindow window =
            _services.GetRequiredService<MainWindow>();

        window.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IAppStateService, AppStateService>();

        services.AddSingleton<MainWindow>();
    }
}