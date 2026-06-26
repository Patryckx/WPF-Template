using Example.Services.Interfaces;
using System.Configuration;
using System.Data;
using System.Windows;
using Example.Services;


namespace Example
{
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            IAppStateService appState = new AppStateService();


            MainWindow window = new MainWindow(appState);

            window.Show();
        }
    }

}
