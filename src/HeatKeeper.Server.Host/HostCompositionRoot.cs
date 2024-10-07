using System.Reflection;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using LightInject;
using Microsoft.AspNetCore.Http;

namespace HeatKeeper.Server.Host
{
    public class HostCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry registry)
        {
            var optionsField = typeof(ServiceContainer).GetField("options", BindingFlags.Instance | BindingFlags.NonPublic);
            var options = (ContainerOptions)optionsField!.GetValue(registry);
            options.EnableVariance = true;
            registry    
                .RegisterSingleton<IUserContext, UserContext>()
                .RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .RegisterFrom<DatabaseCompositionRoot>()
                .RegisterFrom<ServerCompositionRoot>();
        }
    }
}
