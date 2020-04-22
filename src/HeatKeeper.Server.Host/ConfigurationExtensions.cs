using System.Text;
using HeatKeeper.Abstractions.Logging;
using HeatKeeper.Server.Authentication;
using LightInject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using LogLevel = HeatKeeper.Abstractions.Logging.LogLevel;

namespace HeatKeeper.Server.Host
{
    internal static class ConfigurationExtensions
    {
        public static ApplicationConfiguration AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationConfiguration = configuration.Get<ApplicationConfiguration>(c => c.BindNonPublicProperties = true);
            if (string.IsNullOrWhiteSpace(applicationConfiguration.Secret))
            {
                applicationConfiguration.Secret = DefaultSecret.Value;
            }

            services.AddSingleton(applicationConfiguration);

            return applicationConfiguration;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {

                options.AddPolicy("DevelopmentPolicy", config =>
                {
                    config.AllowAnyOrigin();
                    config.AllowAnyMethod();
                    config.AllowAnyHeader();
                });
            });
            return services;
        }


        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, ApplicationConfiguration applicationConfiguration)
        {
            var secret = applicationConfiguration.Secret;

            var key = Encoding.ASCII.GetBytes(applicationConfiguration.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }

        public static IServiceRegistry ConfigureLogging(this IServiceRegistry registry)
        {
            registry.RegisterSingleton<LogFactory>(f =>
            {
                var loggerFactory = f.GetInstance<ILoggerFactory>();
                return type =>
                {
                    var logger = loggerFactory.CreateLogger(type);
                    return (logLevel, message, exception) => logger.Log(MapLogLevel(logLevel), exception, message);
                };
            });
            // TODO Register constructor dependency here.
            return registry;

            static Microsoft.Extensions.Logging.LogLevel MapLogLevel(LogLevel loglevel) => loglevel switch
            {
                LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
                LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
                LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
                LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
                LogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
                _ => Microsoft.Extensions.Logging.LogLevel.Trace
            };
        }
    }
}
