using System.Threading;
using System.Windows;

namespace Prototype
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewModel = new MainWindowViewModel();
            MainWindow = new MainWindow {DataContext = viewModel };
            MainWindow.Show();

            Thread.Sleep(200);

            viewModel.Update();
        }
    }
}
