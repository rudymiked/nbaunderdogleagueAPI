using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using nbaunderdogleagueAPI.Business;
using nbaunderdogleagueAPI.Communications;
using nbaunderdogleagueAPI.DataAccess;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using System.Security.Claims;
using System.Security.Principal;

namespace nbaunderdogleagueAPI
{
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

                services.ConfigureGoogleAuth(
                    Configuration["Authentication:Google:ClientId"],
                    Configuration["Authentication:Google:ClientSecret"]);

                services.AddHttpClient(AppConstants.AppName, client => {
                    client.BaseAddress = new Uri(AppConstants.ApiUrl);
                    client.DefaultRequestHeaders.Add("ContentType", "application/json");
                });

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                services.AddEndpointsApiExplorer();

                services.Configure<AppConfig>(Configuration);

                services.AddAuthorization(options => {
                    Configuration.Bind("AuthorizationClient", options);
                });

                services.AddAuthorization(o => {
                    o.AddPolicy(AppConstants.DefaultAuthPolicy, policy => {
                        // user can't be null
                        policy.RequireAssertion(context => context.User.FindFirst(ClaimTypes.Email) != null);
                    });
                });

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

                services.AddSingleton<IEmailService, EmailService>();
                services.AddSingleton<IEmailRepository, EmailRepository>();
                services.AddSingleton<IEmailHelper, EmailHelper>();

                services.AddSingleton<IInvitationService, InvitationService>();
                services.AddSingleton<IInvitationRepository, InvitationRepository>();
                services.AddSingleton<IInvitationDataAccess, InvitationDataAccess>();

                services.AddSingleton<IPlayerService, PlayerService>();
                services.AddSingleton<IPlayerRepository, PlayerRepository>();
                services.AddSingleton<IPlayerDataAccess, PlayerDataAccess>();

                services.AddSingleton<ITableStorageHelper, TableStorageHelper>();
                services.AddSingleton<IRapidAPIHelper, RapidAPIHelper>();

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NBAUnderdogLeague", Version = "v1" });
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows {
                            Implicit = new OpenApiOAuthFlow {
                                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "email", "Access to your email address" },
                                    { "profile", "Access to your basic profile information" }
                                }
                            }
                        }
                    });
                });

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

    public static class StartupExtentions
    {
        public static void ConfigureGoogleAuth(this IServiceCollection services, string clientId, string clientSecret)
        {
            //services.AddIdentity<IdentityUser, IdentityRole>();
            //services.AddIdentityCore<User>();
            services.AddAuthentication(options => {
                // This forces challenge results to be handled by Google OpenID Handler, so there's no
                // need to add an AccountController that emits challenges for Login.
                options.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                // This forces forbid results to be handled by Google OpenID Handler, which checks if
                // extra scopes are required and does automatic incremental auth.
                options.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
                // Default scheme that will handle everything else.
                // Once a user is authenticated, the OAuth2 token info is stored in cookies.
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddGoogleOpenIdConnect(options => {
                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                });
        }
    }
}