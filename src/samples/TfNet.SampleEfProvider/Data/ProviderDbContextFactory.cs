using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TfNet.SampleEfProvider.Data;

internal class ProviderDbContextFactory : IDesignTimeDbContextFactory<ProviderDbContext>
{
    public ProviderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProviderDbContext>();
        optionsBuilder.UseSqlite(@"Data Source=C:\Projects\_prive\tf.net\src\samples\TfNet.SampleEfProvider\database.db");

        return new ProviderDbContext(optionsBuilder.Options);
    }
}
