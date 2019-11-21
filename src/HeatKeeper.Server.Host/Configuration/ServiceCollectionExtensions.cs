using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeatKeeper.Server.Host.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationConfiguration = configuration.Get<ApplicationConfiguration>(c => c.BindNonPublicProperties = true);
            services.AddSingleton(applicationConfiguration);
            return services;
        }
    }
}
