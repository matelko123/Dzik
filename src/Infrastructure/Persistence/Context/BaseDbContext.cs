using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public class BaseDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    }
}