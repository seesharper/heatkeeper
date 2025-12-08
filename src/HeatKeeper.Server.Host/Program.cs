using CQRS.AspNet;
using HeatKeeper.Abstractions;
using HeatKeeper.Server;
using HeatKeeper.Server.EnergyPrices;
using HeatKeeper.Server.ExchangeRates;
using HeatKeeper.Server.Host;
using HeatKeeper.Server.Host.BackgroundTasks;
using HeatKeeper.Server.Measurements;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using WebPush;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLightInject(services => services.RegisterFrom<HostCompositionRoot>());
builder.Configuration.AddEnvironmentVariables(prefix: "HEATKEEPER_");
// Add services to the container.

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddJanitor();
builder.Services.AddHostedService<MessageBusHostedService>();
builder.Services.AddHostedService<OutdoorLightsHostedService>();
builder.Services.AddHostedService<TriggerEngineHostedService>();
builder.Services.AddHttpClient();
builder.Services.AddCorsPolicy();
builder.Services.AddHttpClient<IWebPushClient, WebPushClient>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSpaStaticFiles(config => config.RootPath = "wwwroot");
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient<NorwegianBankClient>(client => client.BaseAddress = new Uri("https://data.norges-bank.no/api/data/"));
builder.Services.AddHttpClient<EntsoeClient>(client => client.BaseAddress = new Uri("https://web-api.tp.entsoe.eu/api"));



var app = builder.Build();

await app.RunBootStrappers();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevelopmentPolicy");

    app.UseCookiePolicy(new CookiePolicyOptions
    {
        MinimumSameSitePolicy = SameSiteMode.Strict,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always
    });
}
else
{
    app.VerifySecret();
}

app.UseStaticFiles();
app.MapAuthorizeEndpoint();
app.UseAuthorization();
app.UseExceptionHandler(_ => { });


app.MapPost<MeasurementCommand[]>("api/measurements");

app.MapCqrsEndpoints(typeof(ServerCompositionRoot).Assembly);

app.MapControllers();

app.Run();

public partial class Program
{ }
