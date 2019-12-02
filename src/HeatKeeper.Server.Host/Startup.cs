using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Host.Swagger;
using HeatKeeper.Server.Users;
using LightInject;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
            var appConfig = services.AddApplicationConfiguration(Configuration);

            services.AddCorsPolicy();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
                var bodyModelBinderProvider = options.ModelBinderProviders.Single(p => p.GetType() == typeof(BodyModelBinderProvider));
                options.ModelBinderProviders.Insert(0, new RouteAndBodyBinderProvider(bodyModelBinderProvider));
            }).AddControllersAsServices().AddNewtonsoftJson();

            services.AddJwtAuthentication(appConfig);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Heatkeeper", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                //c.OperationFilter<OperationFilter>();
            });
        }

        public void ConfigureContainer(IServiceContainer container)
        {
            container.RegisterSingleton<IUserContext, UserContext>();
            container.RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>();
            container.RegisterFrom<DatabaseCompositionRoot>();
            container.RegisterFrom<ServerCompositionRoot>();
            container.ConfigureLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDatabaseMigrator databaseMigrator)
        {
            databaseMigrator.Migrate();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });

            if (env.IsDevelopment())
            {
                app.UseCors("DevelopmentPolicy");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    // public class CustomModelNameProvider : IModelMetadataProvider
    // {
    //     public IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType)
    //     {
    //         throw new NotImplementedException();
    //     }

    //     public ModelMetadata GetMetadataForType(Type modelType)
    //     {
    //         ModelMetadata metadata;
    //         metadata.de

    //         throw new NotImplementedException();
    //     }
    // }



    public class RouteAndBodyModelBinder : IModelBinder
    {
        private readonly IModelBinder bodyModelBinder;

        public RouteAndBodyModelBinder(IModelBinder bodyModelBinder)
        {
            this.bodyModelBinder = bodyModelBinder;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {

            await bodyModelBinder.BindModelAsync(bindingContext);

            var routeValues = bindingContext.HttpContext.Request.RouteValues.Select(rv => rv.Key);

            var routeProperties = bindingContext.ModelMetadata.Properties.Where(p => routeValues.Contains(p.Name, StringComparer.OrdinalIgnoreCase)).ToArray(); ;
            foreach (var routeProperty in routeProperties)
            {
                var routeValue = bindingContext.ValueProvider.GetValue(routeProperty.Name).FirstValue;
                var convertedValue = Convert.ChangeType(routeValue, routeProperty.UnderlyingOrModelType);
                if (bindingContext.Model == null)
                {
                    bindingContext.Model = Activator.CreateInstance(bindingContext.ModelType);
                }
                routeProperty.PropertySetter(bindingContext.Result.Model, convertedValue);
            }
        }
    }

    public class RouteAndBodyBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinderProvider bodyModelBinderProvider;

        public RouteAndBodyBinderProvider(IModelBinderProvider bodyModelBinderProvider)
        {
            this.bodyModelBinderProvider = bodyModelBinderProvider;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var identity = (ModelMetadataIdentity)typeof(ModelMetadata).GetProperty("Identity", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(context.Metadata);
            if (identity.MetadataKind == ModelMetadataKind.Parameter)
            {
                if (identity.ParameterInfo.IsDefined(typeof(FromBodyAndRouteAttribute)))
                {
                    return new RouteAndBodyModelBinder(bodyModelBinderProvider.GetBinder(context));
                }
            }

            return null;
        }
    }

    public class FromBodyAndRouteAttribute : Attribute
    {

    }
}
