using System.IO;
using System.Windows;

namespace SimpleNetExecutor.Client
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            mainWindow.Show();

            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleNetExecutor")))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleNetExecutor"));

            var updateService = new UpdateService();
            await updateService.CheckForUpdateAsync();

            var executor = new DllExecutor();
            executor.ExecuteLatestDll(text => Dispatcher.Invoke(() => mainWindow.SetText(text)));
        }
    }
}