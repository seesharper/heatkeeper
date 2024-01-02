using System.Text;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace HeatKeeper.Server.Host
{
    internal static class ConfigurationExtensions
    {
        // public static ApplicationConfiguration AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        // {
        //     var applicationConfiguration = configuration.Get<ApplicationConfiguration>(c => c.BindNonPublicProperties = true);
        //     if (string.IsNullOrWhiteSpace(applicationConfiguration.Secret))
        //     {
        //         applicationConfiguration.Secret = DefaultSecret.Value;
        //     }

        //     services.AddSingleton(applicationConfiguration);

        //     return applicationConfiguration;
        // }

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


        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secret = configuration.GetSecret();

            var key = Encoding.ASCII.GetBytes(secret);

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
                    ValidateAudience = false,

                };
            });

            return services;
        }


    }
}
