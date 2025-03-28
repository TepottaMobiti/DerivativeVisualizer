using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace DerivativeVisualizerGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    
    // Csak basic MessageBox handling legyen itt, semmi más nem kell szerintem.
    public partial class App : Application
    {
        private ViewModel viewModel = null!;
        private MainWindow view = null!;
        public App()
        {
            Startup += new StartupEventHandler(App_StartUp);
        }

        private void App_StartUp(object? sender, StartupEventArgs e)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            viewModel = new ViewModel();
            viewModel.ErrorOccurred += (errorMessage) =>
            {
                MessageBox.Show(errorMessage, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            view = new MainWindow();
            view.DataContext = viewModel;
            view.Show();
        }
    }

}
