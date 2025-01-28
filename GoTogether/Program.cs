﻿using Microsoft.IdentityModel.Tokens;
using System.Text;
using GraphQLServer.Services.PlaceService;
using GraphQLServer.Services.RecoveryService;
using GraphQLServer.Services.TripService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Server.Data;
using Server.Services;

namespace GraphQLServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #if DEBUG
                builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            #else
                builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            #endif

            // Настройка авторизации
            builder.Services.AddAuthorization();

            builder.Services.AddScoped<Query>();
            builder.Services.AddScoped<Mutation>();
            builder.Services.AddScoped<Subsription>();
            builder.Services.AddScoped<DataBaseConnection>();
            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ITripService, TripService>();
            builder.Services.AddScoped<IPlaceService, PlaceService>();
            builder.Services.AddSingleton(new ConfigurationHelper(builder.Configuration));


            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMemoryCache();
            builder.Services.AddWebSockets(options => { options.KeepAliveInterval = TimeSpan.FromSeconds(120); });

            builder.Services.AddGraphQLServer()
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
                        OnAuthenticationFailed = context => { return Task.FromResult("AUTH_FAILED_PROBLEM"); }
                    };
                });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000);
                options.ListenAnyIP(5001, listenOptions => { listenOptions.UseHttps(); });
            });


            var app = builder.Build();

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSockets();
            
            
            app.MapGraphQL();
            //app.MapControllers();

            app.Run();
        }
    }
}