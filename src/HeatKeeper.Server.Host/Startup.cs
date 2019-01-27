using System.Diagnostics;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Logging;
using heatkeeper_server.Controllers;
using LightInject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Refit;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddControllersAsServices();
            services.Configure<Settings>(Configuration);
            // var test = Refit.RestService.For<IHelloClient>("http://github.com");

            // var test2 = test.GetType().Assembly.CodeBase;
            ////// How do we secure this API?
            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-custom-storage-providers?view=aspnetcore-2.1

            // https://stackoverflow.com/questions/38661090/token-based-authentication-in-web-api-without-any-user-interface

            // https://developer.okta.com/blog/2018/02/01/secure-aspnetcore-webapi-token-auth




            // configure jwt authentication
            var appSettings = Configuration.Get<Settings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
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



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Heatkeeper", Version = "v1" });
            });
        }

        public void ConfigureContainer(IServiceContainer container)
        {
            container.RegisterScoped<DisposeTest, DisposeTest>();
            container.RegisterFrom<HeatKeeper.Server.Database.CompositionRoot>();
            container.RegisterFrom<HeatKeeper.Server.CompositionRoot>();
            container.RegisterSingleton<LogFactory>(f => {
               var loggerFactory = f.GetInstance<ILoggerFactory>();
               LogFactory factory = (type) => (l, m,e ) => loggerFactory.CreateLogger(type).LogInformation(m);
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
                app.UseDeveloperExceptionPage();
                //app.UseHsts();
            }
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }


    public interface IHelloClient
{
    [Get("/helloworld")]
    Task<Reply> GetMessageAsync();
}

public class Reply
{
    public string Message { get; set; }
}
}
