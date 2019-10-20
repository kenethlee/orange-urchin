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
        }

        private string _token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJvcmdJZCI6IjE4MGRiYTk1LTFhYjYtNDRiMC05Yzk0LTQ2MzBlOGQyODBiZiIsImV4cCI6MTU3MTU2MjI0NSwiaWF0IjoxNTcxNTU4NjQ1fQ.2uYGLq5bSVvCbeF4S2fR-C_AJghV-LWxNQ6hq--Vbw0";
        private string _knowledgeBase = "df599a54-91bd-44c5-9a93-19d055b6ca57";
    }
}
