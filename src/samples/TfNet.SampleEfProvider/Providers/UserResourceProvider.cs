using Microsoft.EntityFrameworkCore;
using TfNet.Extensions;
using TfNet.Models;
using TfNet.Providers.Resource;
using TfNet.SampleEfProvider.Data;
using TfNet.SampleEfProvider.Extensions;
using TfNet.Serialization;

namespace TfNet.SampleEfProvider.Providers;

internal class UserResourceProvider : IResourceProvider<UserResource>
{
    private readonly ProviderDbContext _db;

    public UserResourceProvider(
        ProviderDbContext db)
    {
        _db = db;
    }

    public async Task<UserResource> CreateAsync(UserResource planned)
    {
        var newEntity = new UserEntity
        {
            Name = planned.Name,
            Email = planned.Email,
        };

        UserRoleEntity[]? roles = null;
        if (planned.Roles != null && planned.Roles.Length > 0)
        {
            roles = await _db.Roles.Where(x => planned.Roles.Contains(x.Id)).ToArrayAsync();

            newEntity.Roles = roles;
        }

        var entry = _db.Users.Add(newEntity);

        await _db.SaveChangesAsync();

        return new UserResource
        {
            Id = entry.Entity.Id.ToString(),
            Email = entry.Entity.Email,
            Name = entry.Entity.Name,
            Roles = roles?.Select(x => x.Id).ToArray() ?? []
        };
    }

    public async Task DeleteAsync(UserResource resource)
    {
        var id = int.Parse(resource.Id!);
        await _db.Users.Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    public Task<IList<UserResource>> ImportAsync(string id)
    {
        throw new NotSupportedException();
    }

    public async Task<PlanResult<UserResource>> PlanAsync(UserResource? prior, UserResource proposed)
    {
        var entity = await _db.Users.FirstOrDefaultAsync(r => r.Name == proposed.Name);
        if (entity == null)
        {
            return new(proposed)
            {
                RequiresReplace =
                {
                    AttributePath.For<UserResource>(nameof(UserResource.Name))
                }
            };
        }

        var updatedEntity = new UserResource
        {
            // immutable
            Id = entity.Id.ToString(),
            Name = entity.Name,

            // mutable
            Email = proposed.Email,
            Roles = []
        };

        if (proposed.Roles != null && proposed.Roles.Length > 0)
        {
            var roles = await _db.Roles.Where(x => proposed.Roles.Contains(x.Id)).ToArrayAsync();

            updatedEntity.Roles = roles.Select(x => x.Id).ToArray();
        }

        return new(updatedEntity);
    }

    public async Task<UserResource> ReadAsync(UserResource resource)
    {
        if (resource.Id == null)
        {
            return resource;
        }

        var id = int.Parse(resource.Id);
        var entity = await _db.Users.Include(x => x.Roles).FirstOrDefaultAsync(r => r.Id == id);
        if (entity == null)
        {
            return resource;
        }

        return new UserResource
        {
            Id = entity.Id.ToString(),
            Email = entity.Email,
            Name = entity.Name,
            Roles = entity.Roles.Select(x => x.Id).ToArray()
        };
    }

    public async Task<UserResource> UpdateAsync(UserResource? prior, UserResource planned)
    {
        var id = int.Parse(planned.Id!);
        var entity = await _db.Users.Include(x => x.Roles).FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new InvalidOperationException();

        entity.Name = planned.Name;
        entity.Email = planned.Email;

        var rolesToDelete = entity.Roles.ExceptBy(planned.Roles ?? [], x => x.Id).ToArray();
        foreach (var roleToDelete in rolesToDelete)
        {
            entity.Roles.Remove(roleToDelete);
        }

        if (planned.Roles != null && planned.Roles.Length > 0)
        {
            var rolesToAdd = planned.Roles.Except(entity.Roles.Select(x => x.Id)).ToArray();

            if (rolesToAdd.Length > 0)
            {
                entity.Roles.AddRange(await _db.Roles.Where(x => rolesToAdd.Contains(x.Id)).ToArrayAsync());
            }
        }

        await _db.SaveChangesAsync();

        return new UserResource
        {
            Id = entity.Id.ToString(),
            Email = entity.Email,
            Name = entity.Name,
            Roles = entity.Roles.Select(x => x.Id).ToArray()
        };
    }
}
