using System.Data;
using CQRS.Transactions;
using LightInject;

namespace HeatKeeper.Server.WebApi.Tests.Transactions
{
    public static class ContainerExtensions
    {
        public static IServiceRegistry RegisterRollbackBehavior(this IServiceRegistry serviceRegistry)
        {
            // // Override the IDbConnection registration and change the lifetime from scoped to singleton
            // // so that we use the same connection across all web api calls.
            serviceRegistry.Override(sr => sr.ServiceType == typeof(IDbConnection), (factory, registration) =>
            {
                registration.Lifetime = new PerContainerLifetime();
                return registration;
            });

            // Register the rollback behavior
            serviceRegistry.RegisterSingleton<ICompletionBehavior, RollbackCompletionBehavior>();

            return serviceRegistry;
        }
    }
}
