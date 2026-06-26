using System.Configuration;
using System.Data;
using System.Windows;

namespace modernuitest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var splash = new Splashscreen();
            splash.Show();

            await Task.Delay(1500); // loading simulation

            var intro = new IntroWin
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            intro.Show();

            splash.Close();
        }

        //private async void Application_Startup(object sender, StartupEventArgs e)
        //{
        //    var splash = new IntroWin();
        //    splash.Show();

        //    await Task.Delay(2000);  // simulate loading

        //    var mainWindow = new MainWindow();
        //    mainWindow.Show();

        //    splash.Close();
        //}

    }


}
