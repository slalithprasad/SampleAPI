using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
}
