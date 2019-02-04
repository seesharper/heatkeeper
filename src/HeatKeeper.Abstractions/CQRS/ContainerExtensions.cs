using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightInject;

namespace HeatKeeper.Abstractions.CQRS
{
    /// <summary>
    /// Extends the <see cref="IServiceRegistry"/> interface
    /// </summary>
    public static class ContainerExtensions
    {

        /// <summary>
        /// Registers all implementations of the <see cref="ICommandHandler{TCommand}"/> interface.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <returns></returns>
        public static IServiceRegistry RegisterCommandHandlers(this IServiceRegistry serviceRegistry)
        {
            var commandTypes =
                Assembly.GetCallingAssembly()
                    .GetTypes()
                    .Select(t => GetGenericInterface(t, typeof(ICommandHandler<>)))
                    .Where(m => m != null);
            RegisterHandlers(serviceRegistry, commandTypes);
            serviceRegistry.Register<ICommandExecutor>(factory => new CommandExecutor(GetCurrentScope(factory)), new PerScopeLifetime());
            return serviceRegistry;
        }

        /// <summary>
        /// Registers all implementations of the <see cref="IQueryHandler{TQuery,TResult}"/> interface.
        /// </summary>
        /// <param name="serviceRegistry">The target <see cref="IServiceRegistry"/>.</param>
        /// <returns></returns>
        public static IServiceRegistry RegisterQueryHandlers(this IServiceRegistry serviceRegistry)
        {
            var commandTypes =
                Assembly.GetCallingAssembly()
                    .GetTypes()
                    .Select(t => GetGenericInterface(t, typeof(IQueryHandler<,>)))
                    .Where(m => m != null);
            RegisterHandlers(serviceRegistry, commandTypes);
            serviceRegistry.Register<IQueryExecutor>(factory => new QueryExecutor(GetCurrentScope(factory)), new PerScopeLifetime());
            return serviceRegistry;
        }

        private static IServiceFactory GetCurrentScope(IServiceFactory serviceFactory)
        {
            return ((IServiceContainer)serviceFactory).ScopeManagerProvider.GetScopeManager(serviceFactory).CurrentScope;
        }

        private static (Type, Type)? GetGenericInterface(Type type, Type genericTypeDefinition)
        {
            var closedGenericInterface =
                type.GetInterfaces()
                    .SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeDefinition);
            if (closedGenericInterface != null)
            {
                var constructor = type.GetConstructors().FirstOrDefault();
                if (constructor != null)
                {
                    var isDecorator = constructor.GetParameters().Select(p => p.ParameterType)
                        .Contains(closedGenericInterface);
                    if (!isDecorator)
                    {
                        return (closedGenericInterface, type);
                    }
                }
            }
            return null;
        }

        private static void RegisterHandlers(IServiceRegistry registry, IEnumerable<(Type serviceType, Type implementingType)?> handlers)
        {
            foreach (var handler in handlers)
            {
                if (handler.HasValue)
                {
                    registry.Register(handler.Value.serviceType, handler.Value.implementingType, handler.Value.implementingType.FullName);
                }

            }
        }
    }
}