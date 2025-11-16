using Microsoft.EntityFrameworkCore;

namespace TfNet.SampleEfProvider.Data;

internal class ProviderDbContext : DbContext
{
    public ProviderDbContext(DbContextOptions options) : base(options)
    {
    }

    protected ProviderDbContext()
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;

    public DbSet<UserRoleEntity> Roles { get; set; } = null!;
}
