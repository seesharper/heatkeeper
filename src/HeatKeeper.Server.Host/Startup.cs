using HeatKeeper.Server.Database;
using HeatKeeper.Server.Logging;
using LightInject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // How do we secure this API?
            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-custom-storage-providers?view=aspnetcore-2.1

            // https://stackoverflow.com/questions/38661090/token-based-authentication-in-web-api-without-any-user-interface

            // https://developer.okta.com/blog/2018/02/01/secure-aspnetcore-webapi-token-auth
        }

        public void ConfigureContainer(IServiceContainer container)
        {
            container.RegisterFrom<HeatKeeper.Server.Database.CompositionRoot>();
            container.RegisterFrom<HeatKeeper.Server.CompositionRoot>();
            container.RegisterSingleton<LogFactory>(f => {
               var loggerFactory = f.GetInstance<ILoggerFactory>();
               LogFactory factory = (type) => (l,m,e) => loggerFactory.CreateLogger(type).LogInformation("d");
               return factory;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDatabaseInitializer databaseInitializer)
        {
            var test = Configuration.GetValue<string>("ConnectionString");
            databaseInitializer.Initialize(test);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
