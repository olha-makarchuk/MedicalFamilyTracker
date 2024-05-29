using MedicalFamilyTracker.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MedicalFamilyTracker
{
    public partial class App : Application
    {
        readonly MainWindow mainWindow;
        public App(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //MainWindowViewModel viewModel = new   MainWindowViewModel(_context, mainWindow);

            //mainWindow.DataContext = viewModel;

            mainWindow.Show();
            base.OnStartup(e);
        }

        public static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddScoped<App>();
            services.AddScoped<MainWindow>();
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(host.Configuration.GetConnectionString("DefaultConnection"));
            });
        }

        public static string GetCurrentDirectory() => Path.GetDirectoryName(GetSourceCodePath()) ?? Environment.CurrentDirectory;

        public static string GetSourceCodePath([CallerFilePath] string path = null) => path;
    }
}
