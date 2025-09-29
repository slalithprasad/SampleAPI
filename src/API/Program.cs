using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using API.Filters;
using API.Middlewares;
using NLog;
using NLog.Web;
using API.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using NSwag;
using NSwag.Generation.Processors.Security;
using NSwag.AspNetCore;
using API.OpenApi;

try
{
    var builder = WebApplication.CreateBuilder(args);

    if (builder.Environment.IsDevelopment())
    {
        DotNetEnv.Env.Load();
    }

    builder.Configuration
        .AddJsonFile("nlog.settings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    string storageConnectionString = builder.Configuration["STORAGE_CONNECTION_STRING"]!;

    builder.Services.AddHealthChecks()
        .AddAzureBlobStorage(storageConnectionString, name: "blob-storage",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "storage", "critical" })
        .AddCheck("in-memory-db", () => HealthCheckResult.Healthy("In-memory DB is always available"));

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

    builder.Services.AddAuthorization();
    builder.Services.AddAuthentication();

    builder.Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
        options.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.UrlSegmentApiVersionReader();
    });

    builder.Services.AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";  // v1, v2, etc.
        options.SubstituteApiVersionInUrl = true;
    });

    builder.Services.AddOpenApiDocument(config =>
    {
        config.DocumentName = "v1";
        config.Title = "ToDo API - v1";
        config.Version = "1.0";
        config.Description = "An ASP.NET Core Web API for managing ToDo items";

        config.ApiGroupNames = new[] { "v1" };

        config.OperationProcessors.Insert(0, new AspNetCoreOperationSecurityScopeProcessor("JWT"));

        config.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
        {
            Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = NSwag.OpenApiSecurityApiKeyLocation.Header,
            Description = "Enter 'Bearer {token}'"
        });

        config.OperationProcessors.Add(new CustomOpenApiProcessor());

        config.PostProcess = document =>
        {
            document.Info = new OpenApiInfo
            {
                TermsOfService = "https://example.com/terms",
                Contact = new OpenApiContact
                {
                    Name = "API Team",
                    Url = "https://example.com/contact",
                    Email = "support@example.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = "https://opensource.org/licenses/MIT"
                }
            };

            foreach (var path in document.Paths)
            {
                foreach (var opPair in path.Value)
                {
                    var operation = opPair.Value;

                    if (operation.Security != null && operation.Security.Any())
                        continue;

                    if (operation.Responses != null && (operation.Responses.ContainsKey("401") || operation.Responses.ContainsKey("403")))
                    {
                        operation.Security ??= new System.Collections.Generic.List<NSwag.OpenApiSecurityRequirement>();
                        operation.Security.Add(new NSwag.OpenApiSecurityRequirement { { "JWT", new string[] { } } });
                    }
                }
            }
        };
    });


    builder.Services.AddOpenApiDocument(config =>
    {
        config.DocumentName = "v2";
        config.Title = "ToDo API - v2";
        config.Version = "2.0";
        config.Description = "An ASP.NET Core Web API for managing ToDo items";

        config.ApiGroupNames = new[] { "v2" };

        config.OperationProcessors.Insert(0, new AspNetCoreOperationSecurityScopeProcessor("JWT"));

        config.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
        {
            Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = NSwag.OpenApiSecurityApiKeyLocation.Header,
            Description = "Enter 'Bearer {token}'"
        });

        config.OperationProcessors.Add(new CustomOpenApiProcessor());

        config.PostProcess = document =>
        {
            document.Info = new OpenApiInfo
            {
                TermsOfService = "https://example.com/terms",
                Contact = new OpenApiContact
                {
                    Name = "API Team",
                    Url = "https://example.com/contact",
                    Email = "support@example.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = "https://opensource.org/licenses/MIT"
                }
            };

            foreach (var path in document.Paths)
            {
                foreach (var opPair in path.Value)
                {
                    var operation = opPair.Value;

                    if (operation.Security != null && operation.Security.Any())
                        continue;

                    if (operation.Responses != null && (operation.Responses.ContainsKey("401") || operation.Responses.ContainsKey("403")))
                    {
                        operation.Security ??= new System.Collections.Generic.List<NSwag.OpenApiSecurityRequirement>();
                        operation.Security.Add(new NSwag.OpenApiSecurityRequirement { { "JWT", new string[] { } } });
                    }
                }
            }
        };
    });

    var app = builder.Build();

    // Serve JSON docs
    app.UseOpenApi(settings =>
    {
        settings.Path = "/swagger/{documentName}/swagger.json";

        settings.PostProcess = (doc, request) =>
        {
            doc.Servers.Clear();

            doc.Servers.Add(new NSwag.OpenApiServer
            {
                Url = "http://localhost:5072",
                Description = "Development"
            });

            doc.Servers.Add(new NSwag.OpenApiServer
            {
                Url = "https://uat.example.com",
                Description = "UAT"
            });

            doc.Servers.Add(new NSwag.OpenApiServer
            {
                Url = "https://api.example.com",
                Description = "Production"
            });
        };
    });

    // Serve Swagger UI
    app.UseSwaggerUi(settings =>
    {
        settings.Path = "/swagger";
        settings.DocumentPath = "/swagger/{documentName}/swagger.json";

        settings.SwaggerRoutes.Add(new SwaggerUiRoute("API v1", "/swagger/v1/swagger.json"));
        settings.SwaggerRoutes.Add(new SwaggerUiRoute("API v2", "/swagger/v2/swagger.json"));
    });

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

    app.UseAuthentication();
    app.UseAuthorization();

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
