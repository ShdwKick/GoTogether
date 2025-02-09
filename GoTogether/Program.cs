using System.Text;
using GoTogether.Data;
using GoTogether.Repositories;
using GoTogether.Repositories.TripRepositories;
using GoTogether.Repositories.UserRepository;
using GoTogether.Services.DatabaseInitializerService;
using GoTogether.Services.RecoveryService;
using GoTogether.Services.RoleService;
using GoTogether.Services.TripService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Data.Helpers;
using Server.Services;

namespace GoTogether
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

#if DEBUG
            builder.Configuration.AddJsonFile("Properties/appsettings.Development.json", optional: false, reloadOnChange: true);
#else
                builder.Configuration.AddJsonFile("Properties/appsettings.json", optional: false, reloadOnChange: true);
#endif

            // Настройка авторизации
            builder.Services.AddAuthorization();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMemoryCache();
            builder.Services.AddWebSockets(options => { options.KeepAliveInterval = TimeSpan.FromSeconds(120); });

            builder.Services.AddHostedService<DatabaseInitializerService>();

            builder.Services.AddScoped<Query>();
            builder.Services.AddScoped<Mutation>();
            builder.Services.AddScoped<Subsription>();
            builder.Services.AddScoped<DatabaseConnection>();
            
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITripRepository, TripRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<ILandmarkRepository, LandmarkRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ITripService, TripService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddSingleton(new ConfigurationHelper(builder.Configuration));


            

            builder.Services.AddGraphQLServer()
                .ModifyRequestOptions(options =>
                {
                    //ограничение на максимальное время запроса
                    options.ExecutionTimeout = TimeSpan.FromSeconds(60);
                })
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddSubscriptionType<Subsription>()
                .AddInMemorySubscriptions()
                .AddAuthorization();

            //builder.Services.AddControllers();

            var key = ConfigurationHelper.GetServerKey();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Настройка аутентификации JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = securityKey,
                        ValidateIssuer = true,
                        ValidIssuer = ConfigurationHelper.GetIssuer(),
                        ValidateAudience = true,
                        ValidAudience = ConfigurationHelper.GetAudience(),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = _ => Task.FromResult("AUTH_FAILED_PROBLEM")
                    };
                });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5012);
                options.ListenAnyIP(5001, listenOptions => { listenOptions.UseHttps(); });
            });


            var app = builder.Build();

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSockets();


            app.MapGraphQL();
            //app.MapControllers();

            //set response max size limits 
            app.Use(async (context, next) =>
            {
                var originalBody = context.Response.Body;
                await using var limitedStream = new MemoryStream();
                context.Response.Body = limitedStream;

                await next();

                if (limitedStream.Length > (1024 * 1024) * 3) // 3MB
                {
                    context.Response.StatusCode = StatusCodes.Status413RequestEntityTooLarge;
                    await context.Response.WriteAsync("Response too large");
                }
                else
                {
                    limitedStream.Seek(0, SeekOrigin.Begin);
                    await limitedStream.CopyToAsync(originalBody);
                }

                context.Response.Body = originalBody;
            });
            
            Helpers.InitializeServiceProvider(app.Services);

            app.Run();
        }
    }
}