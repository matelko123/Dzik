using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : BaseDbContext(options)
{
    
    // Here add application db sets like:
    // public DbSet<Product> Products => Set<Product>();
};