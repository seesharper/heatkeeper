using System.Data;
using System.Linq;
using HeatKeeper.Server.CQRS;
using LightInject;

namespace HeatKeeper.Server.WebApi.Tests.Transactions
{
    public static class ContainerExtensions
    {
        public static void EnableRollback(this IServiceContainer container)
        {
            var connectionRegistration = container.AvailableServices.Where(sr => sr.ServiceType == typeof(IDbConnection)).FirstOrDefault();
            connectionRegistration.Lifetime = new PerContainerLifetime();
            container.Decorate(typeof(ICommandHandler<>), typeof(RollbackCommandHandler<>));
        }
    }
}