using HeatKeeper.Server.Host;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLightInject(services => services.RegisterFrom<HostCompositionRoot>());
builder.Configuration.AddEnvironmentVariables(prefix: "HEATKEEPER_");
// Add services to the container.

builder.Services.AddJanitor();
builder.Services.AddHttpClient();
builder.Services.AddCorsPolicy();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSpaStaticFiles(config => config.RootPath = "wwwroot");
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddMvc(options =>
{
    options.Filters.Add<DeleteActionFilter>();
    var bodyModelBinderProvider = options.ModelBinderProviders.Single(p => p.GetType() == typeof(BodyModelBinderProvider));
    options.ModelBinderProviders.Insert(0, new RouteAndBodyBinderProvider(bodyModelBinderProvider));
});


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
// app.UseSpaStaticFiles();
// app.UseSpa(config => config.Options.SourcePath = "wwwroot");
app.UseAuthorization();

app.UseExceptionHandler(_ => { });

app.MapControllers();

app.Run();

public partial class Program
{ }