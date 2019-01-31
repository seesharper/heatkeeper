using System.Diagnostics;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Logging;
using LightInject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HeatKeeper.Server.Users;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddControllersAsServices();
            services.Configure<Settings>(Configuration);


            // configure jwt authentication
            var appSettings = Configuration.Get<Settings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var test = Encoding.UTF8.GetBytes(appSettings.Secret);


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
            }).AddCookie();



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Heatkeeper", Version = "v1" });
            });
        }

        public void ConfigureContainer(IServiceContainer container)
        {
            container.Register<IUserContext, UserContext>();
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




            app.UseAuthentication();



            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }



}
