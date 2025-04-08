using SmartSpin.Properties;
using SmartSpin.View;
using System.Configuration;
using System.IO;
using System.Windows;
using SmartSpin.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using SmartSpin.Hardware;
using MintControls_5864Lib;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SmartSpin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private IServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            logger.Info("App_Startup");

            // Load the splash screen
            Startup startup = new Startup();
            startup.Show();

            try
            {
                Settings.Default.AntiCorruptionTest = true;
            }
            catch (ConfigurationErrorsException ex)
            {
                string filename = ((ConfigurationErrorsException)ex.InnerException).Filename;
                File.Delete(filename);
                this.Shutdown();
                return;
            }

            try
            {
                logger.Info("Connect to machine");

                // Do the processing of connecting to the machine
                startup.TheMachineConnect(_serviceProvider);

                logger.Info("Create Main Window");

                // Load your windows but don't show it yet
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                App.Current.MainWindow = mainWindow;

                App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

                mainWindow.Show();
            }
            finally
            {
                startup.Close();
            }

        }

        [SupportedOSPlatform("windows")]
        private static readonly Type ControllerCLSID = Marshal.GetTypeFromCLSID(new Guid("054561BC-015D-47BA-ACDA-D891F04BC28A"));

        [SupportedOSPlatform("windows")]
        public static MintController CreateMintController()
        {
            return (MintController?)Activator.CreateInstance(ControllerCLSID) ?? throw new NullReferenceException("Unable to create Mint Controller object");
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();

            // Configure Logging
            //services.AddLogging();
            services.AddLogging(loggingBuilder =>
             {
                 // configure Logging with NLog
                 loggingBuilder.ClearProviders();
                 loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                 loggingBuilder.AddNLog(config);
             });

            // Register Services
            services.AddTransient<MintController>(provider => CreateMintController());

            services.AddTransient<Controller>();

            // Register ViewModels
            services.AddSingleton<MainViewModel>();

            // Register Views
            services.AddSingleton<MainWindow>();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            // Dispose of services if needed
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            NLog.LogManager.Shutdown();
        }

        //void Application_Startup(object sender, StartupEventArgs e)
        //{
        //    logger.Info("App_Startup");

        //    // Load the splash screen
        //    Startup startup = new Startup();
        //    startup.Show();

        //    try
        //    {
        //        Settings.Default.AntiCorruptionTest = true;
        //    }
        //    catch (ConfigurationErrorsException ex)
        //    {
        //        string filename = ((ConfigurationErrorsException)ex.InnerException).Filename;
        //        File.Delete(filename);
        //        this.Shutdown();
        //        return;
        //    }

        //    try
        //    {
        //        // Do the processing of connecting to the machine
        //        startup.TheMachineConnect(_serviceProvider);

        //        logger.Info("Create Main Window");
        //        // Load your windows but don't show it yet
        //        var main = _serviceProvider.GetRequiredService<MainWindow>();
        //        App.Current.MainWindow = main;

        //        App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

        //        // Show the page
        //        main.Show();

        //    }
        //    finally
        //    {
        //        startup.Close();
        //    }
        //}
    }
}
