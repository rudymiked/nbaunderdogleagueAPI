
using Azure.Core;
using Azure.Identity;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                webBuilder.ConfigureAppConfiguration((hostContext, config) => {
                    var builder = new ConfigurationBuilder()
                    .AddEnvironmentVariables();

                    IConfigurationRoot configuration = builder.Build();

                    var settings = config.Build();

                    string connection = settings.GetConnectionString(AppConstants.NBAFantasyConfig);

#if DEBUG   
                    connection = settings[AppConstants.NBAFantasyConfigLocal];
#endif

                    TokenCredential credentials = new DefaultAzureCredential();

                    config.AddAzureAppConfiguration(options => {
                        options.Connect(connection)
                        .ConfigureKeyVault(kv => {
                            kv.SetCredential(credentials);
                        });
                    });

                    var env = hostContext.HostingEnvironment;

                    config.AddJsonFile("application/json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                }).UseStartup<Startup>());
        }
    }
}