using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HeatKeeper.Server.Host
{
    public static class ConfigurationExtensions
    {
        public static ApplicationConfiguration AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationConfiguration = configuration.Get<ApplicationConfiguration>(c => c.BindNonPublicProperties = true);
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
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("Unable to find secret");
            }

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
    }
}
