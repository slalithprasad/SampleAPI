using Business.Interfaces;
using Business.Managers;
using Domain.Contexts;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("AppDb"));

        services.AddScoped<IOrderManager, OrderManager>();

        return services;
    }
}
