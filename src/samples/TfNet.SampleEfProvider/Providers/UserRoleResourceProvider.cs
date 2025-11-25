using Microsoft.EntityFrameworkCore;
using TfNet.Extensions;
using TfNet.Models;
using TfNet.Providers.Resource;
using TfNet.SampleEfProvider.Data;
using TfNet.SampleEfProvider.Extensions;
using TfNet.Serialization;

namespace TfNet.SampleEfProvider.Providers;

internal class UserRoleResourceProvider : IResourceProvider<UserRoleResource>
{
    private readonly ProviderDbContext _db;

    public UserRoleResourceProvider(
        ProviderDbContext db)
    {
        _db = db;
    }

    public async Task<UserRoleResource> CreateAsync(UserRoleResource planned)
    {
        var entry = _db.Roles.Add(new UserRoleEntity
        {
            RoleName = planned.RoleName,
        });

        await _db.SaveChangesAsync();

        return new UserRoleResource
        {
            Id = entry.Entity.Id,
            RoleName = planned.RoleName,
        };
    }

    public async Task DeleteAsync(UserRoleResource resource)
    {
        await _db.Roles.Where(x => x.Id == resource.Id).ExecuteDeleteAsync();
    }

    public Task<IList<UserRoleResource>> ImportAsync(string id)
    {
        throw new NotSupportedException();
    }

    public async Task<PlanResult<UserRoleResource>> PlanAsync(UserRoleResource? prior, UserRoleResource proposed)
    {
        var entity = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == proposed.RoleName);
        if (entity == null)
        {
            return new(proposed)
            {
                RequiresReplace =
                {
                    AttributePath.For<UserRoleResource>(nameof(UserRoleResource.RoleName))
                }
            };
        }

        return new(new UserRoleResource
        {
            // immutable
            Id = entity.Id,
            RoleName = entity.RoleName
        });
    }

    public async Task<UserRoleResource> ReadAsync(UserRoleResource resource)
    {
        if (resource.Id == null)
        {
            return resource;
        }

        var entity = await _db.Roles.FirstOrDefaultAsync(r => r.Id == resource.Id);
        if (entity == null)
        {
            return resource;
        }

        return new UserRoleResource
        {
            Id = entity.Id,
            RoleName = entity.RoleName,
        };
    }

    public async Task<UserRoleResource> UpdateAsync(UserRoleResource? prior, UserRoleResource planned)
    {
        var entity = await _db.Roles.FirstOrDefaultAsync(r => r.Id == planned.Id)
            ?? throw new InvalidOperationException();

        entity.RoleName = planned.RoleName;

        await _db.SaveChangesAsync();

        return new UserRoleResource
        {
            Id = entity.Id,
            RoleName = entity.RoleName,
        };
    }
}
