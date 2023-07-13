using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using System.Configuration;

namespace Chain
{
    // Path.GetDirectoryName(Application.ExecutablePath).Replace(@"bin\debug\", string.Empty);
    internal static class Program
    {
        public static IConfiguration Configuration;
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //// To customize application configuration such as set high DPI settings or default font,
            //// see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());


            // https://stackoverflow.com/questions/65669920/adding-configuration-to-windows-forms-on-net-5-0
            // https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#memory-configuration-provider
            // https://stackoverflow.com/questions/70475830/how-to-use-dependency-injection-in-winforms

            string databaseFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "jobs.db");

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            // load in the json file as configuration
            //.AddJsonFile(path: "myconfig.json", optional: false, reloadOnChange: true)
            // override configuration from the json file with commandline arguments
            .AddCommandLine(args)

            //               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["LiteDB:ConnectionString"] = databaseFileName,
                    ["Logging:LogLevel:Default"] = "Warning"
                })
            ;
            Configuration = builder.Build();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ServiceProvider = CreateHostBuilder().Build().Services;

            Form1 form = ServiceProvider.GetRequiredService<Form1>();
            Application.Run(form);
        }

        // https://stackoverflow.com/questions/70475830/how-to-use-dependency-injection-in-winforms
        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {
                    services.AddSingleton<IConfiguration>(Configuration);
                    services.AddFeatureManagement(Configuration.GetSection("MyFeatureFlags"));
                    services.AddSingleton<IChainLinkDataService, ChainLinkDataService>();
                    services.AddTransient<Form1>();
                });
        }
    }
}