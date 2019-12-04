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
            registry
                .RegisterSingleton<IUserContext, UserContext>()
                .RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .RegisterFrom<DatabaseCompositionRoot>()
                .RegisterFrom<ServerCompositionRoot>()
                .ConfigureLogging();
        }
    }
}
