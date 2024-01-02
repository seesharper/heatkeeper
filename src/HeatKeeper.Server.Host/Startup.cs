using System.Reflection;
using CQRS.Command.Abstractions;
using HeatKeeper.Abstractions;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Electricity;
using HeatKeeper.Server.Host.BackgroundTasks;
using HeatKeeper.Server.Host.Swagger;
using HeatKeeper.Server.Measurements;
using HeatKeeper.Server.Programs;
using Janitor;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
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
            services.AddJanitor((sp, config) => config
                .Schedule(builder => builder
                    .WithName("ExportElectricalMarketPrices")
                    .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken cancellationToken)
                        => await commandExecutor.ExecuteAsync(new ExportAllMarketPricesCommand(), cancellationToken))
                    .WithSchedule(new CronSchedule("0 15,18,21 * * *")))
                .Schedule(builder => builder
                    .WithName("ExportMeasurements")
                    .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken cancellationToken)
                        => await commandExecutor.ExecuteAsync(new ExportMeasurementsCommand(), cancellationToken))
                    .WithSchedule(new CronSchedule("* * * * *")))
                .Schedule(builder => builder
                    .WithName("SetChannelStates")
                    .WithScheduledTask(async (ICommandExecutor commandExecutor, CancellationToken cancellationToken)
                        => await commandExecutor.ExecuteAsync(new SetChannelStatesCommand(), cancellationToken))
                    .WithSchedule(new CronSchedule(Configuration.GetChannelStateCronExpression())))
            );

            services.AddHostedService<JanitorHostedService>();
            services.AddHttpClient();

            services.AddCorsPolicy();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
                options.Filters.Add<DeleteActionFilter>();
                var bodyModelBinderProvider = options.ModelBinderProviders.Single(p => p.GetType() == typeof(BodyModelBinderProvider));
                options.ModelBinderProviders.Insert(0, new RouteAndBodyBinderProvider(bodyModelBinderProvider));
            }).AddControllersAsServices().AddNewtonsoftJson();

            services.AddJwtAuthentication(Configuration);
            services.AddSpaStaticFiles(config => config.RootPath = "wwwroot");
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Heatkeeper", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.OperationFilter<OperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDatabaseMigrator databaseMigrator, IEnumerable<IBootStrapper> bootstrappers)
        {




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

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseSpa(config => config.Options.SourcePath = "wwwroot");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    
}
