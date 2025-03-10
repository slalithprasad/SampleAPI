using Business.Interfaces;
using Business.Managers;
using Domain.Contexts;
using Integration.Interfaces;
using Integration.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("AppDb"));

        services.AddManagers();
        services.AddIntegrations();

        return services;
    }

    private static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<IOrderManager, OrderManager>();

        return services;
    }

    private static IServiceCollection AddIntegrations(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }
}
