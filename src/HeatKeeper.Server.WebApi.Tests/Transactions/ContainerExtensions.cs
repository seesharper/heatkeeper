using System.Data;
using System.Linq;
using CQRS.Transactions;
using LightInject;

namespace HeatKeeper.Server.WebApi.Tests.Transactions
{
    public static class ContainerExtensions
    {
        public static IServiceRegistry RegisterRollbackBehavior(this IServiceRegistry serviceRegistry)
        {
            // Find the IDbConnection registration and change the lifetime from scoped to singleton
            // so that we use the same connection across all web api calls.
            var dbConnectionRegistration = serviceRegistry.AvailableServices
                .Single(sr => sr.ServiceType == typeof(IDbConnection));
            dbConnectionRegistration.Lifetime = new PerContainerLifetime();

            // Register the rollback behavior
            serviceRegistry.RegisterSingleton<ICompletionBehavior, RollbackCompletionBehavior>();

            return serviceRegistry;
        }
    }
}
