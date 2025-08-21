using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using API.Filters;
using API.Middlewares;
using NLog;
using NLog.Web;
using API.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Scalar.AspNetCore;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    if (builder.Environment.IsDevelopment())
    {
        DotNetEnv.Env.Load();
    }

    builder.Configuration.AddJsonFile("nlog.settings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();

    string storageConnectionString = builder.Configuration["STORAGE_CONNECTION_STRING"]!;

    builder.Services.AddHealthChecks().AddAzureBlobStorage(storageConnectionString, name: "blob-storage", failureStatus: HealthStatus.Unhealthy, tags: new[] { "storage", "critical" }).AddCheck("in-memory-db",
        () => HealthCheckResult.Healthy("In-memory DB is always available"));;

    GlobalDiagnosticsContext.Set("StorageConnectionString", storageConnectionString);

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

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHttpClient();

    builder.Services.AddResponseCompression();

    builder.Services.AddResponseCaching();

    builder.Services.AddServices();

    builder.Services.AddOpenApi();

    WebApplication app = builder.Build();

    app.MapOpenApi();

    app.MapScalarApiReference();

    if (!app.Environment.IsDevelopment())
    {
        var headers = new HeaderPolicyCollection()
        .AddContentTypeOptionsNoSniff()
        .AddReferrerPolicyStrictOriginWhenCrossOrigin()
        .AddFrameOptionsDeny()
        .AddContentSecurityPolicy(csp =>
        {
            csp.AddDefaultSrc().Self();
            csp.AddScriptSrc().Self();
            csp.AddStyleSrc().Self().UnsafeInline();
            csp.AddImgSrc().Self().Data();
            csp.AddConnectSrc().Self();
            csp.AddFrameAncestors().None();
            csp.AddObjectSrc().None();
        })
        .RemoveServerHeader();

        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseSecurityHeaders(headers);
    }

    app.UseResponseCaching();
    app.UseStaticFiles();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseResponseCompression();

    if (app.Environment.IsDevelopment())
    {
        app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    }

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
