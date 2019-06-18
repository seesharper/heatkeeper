using System.Diagnostics;
using System.Threading.Tasks;
using HeatKeeper.Server.Database;
using HeatKeeper.Abstractions.Logging;
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
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

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
            services.AddCors(options =>
            {

                options.AddPolicy("DevelopmentPolicy", config =>
                {
                    config.AllowAnyOrigin();
                    config.AllowAnyMethod();
                    config.AllowAnyHeader();
                });
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddControllersAsServices();
            services.Configure<Settings>(Configuration);

            // configure jwt authentication
            var appSettings = Configuration.Get<Settings>();
            var secret = appSettings.Secret;
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("Unable to find secret");
            }

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
            container.RegisterSingleton<IUserContext, UserContext>();
            container.RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>();
            container.RegisterFrom<HeatKeeper.Server.Database.CompositionRoot>();
            container.RegisterFrom<HeatKeeper.Server.CompositionRoot>();
            container.RegisterSingleton<LogFactory>(f =>
            {
                var loggerFactory = f.GetInstance<ILoggerFactory>();
                LogFactory factory = (type) => (l, m, e) => loggerFactory.CreateLogger(type).LogInformation(m);
                return factory;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDatabaseInitializer databaseInitializer)
        {
            app.UseStaticFiles();
            databaseInitializer.Initialize();


            if (env.IsDevelopment())
            {
                app.UseCors("DevelopmentPolicy");
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
                c.RoutePrefix = "swagger";
            });

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }



}
