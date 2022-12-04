using Microsoft.Extensions.Logging.ApplicationInsights;
using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using System.Security.Principal;

public class Startup
{
    public IConfiguration Configuration { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        try {
            services.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);
            services.AddHttpContextAccessor();
            services.AddControllers();
            services.AddLogging(configure => {
                configure.AddApplicationInsights(Configuration[AppConstants.NBAAppInsights]);
                configure.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information)
                .AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Error);
            });

            services.AddCors();
            services.AddMvc();
            services.AddAzureAppConfiguration();

            services.AddHttpClient(AppConstants.AppName, client => {
                client.BaseAddress = new Uri(AppConstants.ApiUrl);
                client.DefaultRequestHeaders.Add("ContentType", "application/json");
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.Configure<AppConfig>(Configuration);

            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IUserDataAccess, UserDataAccess>();

            services.AddSingleton<ITeamService, TeamService>();
            services.AddSingleton<ITeamRepository, TeamRepository>();
            services.AddSingleton<ITeamDataAccess, TeamDataAccess>();

            services.AddSingleton<IGroupService, GroupService>();
            services.AddSingleton<IGroupRepository, GroupRepository>();
            services.AddSingleton<IGroupDataAccess, GroupDataAccess>();

            services.AddSingleton<IDraftService, DraftService>();
            services.AddSingleton<IDraftRepository, DraftRepository>();
            services.AddSingleton<IDraftDataAccess, DraftDataAccess>();

            services.AddSingleton<IArchiveService, ArchiveService>();
            services.AddSingleton<IArchiveRepository, ArchiveRepository>();
            services.AddSingleton<IArchiveDataAccess, ArchiveDataAccess>();

            services.AddSingleton<INBAService, NBAService>();
            services.AddSingleton<INBARepository, NBARepository>();
            services.AddSingleton<INBADataAccess, NBADataAccess>();

            services.AddSingleton<ITableStorageHelper, TableStorageHelper>();

            services.AddSwaggerGen();
        } catch (Exception ex) {
            Console.WriteLine("Startup Configure Services: " + ex.Message);
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        try {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAzureAppConfiguration();

            app.UseCors(p => {
                p
                .WithOrigins("http://localhost:3000", "https://localhost:7161")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        } catch (Exception ex) {
            Console.WriteLine("Startup Configure: " + ex.Message);
        }
    }
}