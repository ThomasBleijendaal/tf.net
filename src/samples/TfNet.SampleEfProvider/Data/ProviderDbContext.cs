using Microsoft.EntityFrameworkCore;

namespace TfNet.SampleEfProvider.Data;

internal class ProviderDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; } = null!;

    public DbSet<UserRoleEntity> Roles { get; set; } = null!;
}
