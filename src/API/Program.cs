using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using API.Filters;
using API.Middlewares;
using NLog;
using NSwag;
using NLog.Web;
using API.Extensions;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    var config = builder.Configuration;
    var storageConnectionString = config["StorageConnectionString"];
    GlobalDiagnosticsContext.Set("StorageConnectionString", storageConnectionString);

    LogManager.Setup().LoadConfigurationFromXml("NLog.config");

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    builder.Host.UseNLog();

    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(60);
    });

    builder.Services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = true;
    });

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidateModelFilter>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    builder.Services.AddOpenApiDocument(options =>
   {
       options.PostProcess = document =>
       {
           document.Info = new OpenApiInfo
           {
               Version = "v1",
               Title = "DeMeNew Inventory API",
               Description = "An ASP.NET 9 Web API for DeMeNew Inventory",
           };
       };

       options.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
       {
           Type = OpenApiSecuritySchemeType.ApiKey,
           Name = "Authorization",
           In = OpenApiSecurityApiKeyLocation.Header,
           Description = "Type into the textbox: 'Bearer {your JWT token}'."
       });
   });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient();

    builder.Services.AddResponseCompression();

    builder.Services.AddHealthChecks();

    builder.Services.AddServices();

    WebApplication app = builder.Build();

    app.UseOpenApi();

    app.UseSwaggerUi();

    app.UseReDoc(options =>
    {
        options.Path = "/redoc";
    });

#if !DEBUG
        app.UseHsts();
        app.UseHttpsRedirection();
#endif

    app.UseStaticFiles();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseResponseCompression();

#if DEBUG
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
#endif

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.MapGet("/", () => "API is Operational");

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    var logger = LogManager.GetCurrentClassLogger();
    logger.Error(ex, "an error occured: {message}", ex.Message);
    throw;
}
finally
{
    LogManager.Shutdown();
}