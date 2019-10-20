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

            var kb = new GenesysKnowledgeBase(_token, _knowledgeBase);
            var viewModel = new MainWindowViewModel(kb);
            MainWindow = new MainWindow {DataContext = viewModel };
            MainWindow.Show();

            viewModel.Update();
        }

        private string _token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJvcmdJZCI6IjE4MGRiYTk1LTFhYjYtNDRiMC05Yzk0LTQ2MzBlOGQyODBiZiIsImV4cCI6MTU3MTU0NzMyOSwiaWF0IjoxNTcxNTQzNzI5fQ.7SgahurxtRh3V3DESCJfU-Akn0HJPPpN38Fjml1DPSI";
        private string _knowledgeBase = "df599a54-91bd-44c5-9a93-19d055b6ca57";
    }
}
