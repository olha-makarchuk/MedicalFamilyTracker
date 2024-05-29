using MedicalFamilyTracker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SimpleTodoDb
{
	public class Program
	{
		[STAThread]
		public static void Main()
		{
			var host = Host.CreateDefaultBuilder()
				.UseContentRoot(App.GetCurrentDirectory())
			    .ConfigureAppConfiguration((host, cfg) =>
					cfg.SetBasePath(App.GetCurrentDirectory())
					   .AddJsonFile("appsettings.json", true, true))
			    .ConfigureServices(App.ConfigureServices)
				.Build();

			var app = host.Services.GetService<App>();
			app?.Run();
		}
	}
}
