using MessagePack;
using Microsoft.EntityFrameworkCore;
using TfNet.Extensions;
using TfNet.Models;
using TfNet.Providers.Resource;
using TfNet.Resources;
using TfNet.SampleEfProvider.Data;
using TfNet.Serialization;

namespace TfNet.SampleEfProvider.Providers;

[SchemaVersion(1)]
[MessagePackObject]
public class UserResource
{
    [Key("id")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedIntValueFormatter))]
    public int? Id { get; set; }

    [Key("name")]
    public string? Name { get; set; }

    [Key("email")]
    public string? Email { get; set; }
}

[SchemaVersion(1)]
[MessagePackObject]
public class UserRoleResource
{
    [Key("id")]
    [Computed]
    [MessagePackFormatter(typeof(ComputedIntValueFormatter))]
    public int? Id { get; set; }

    [Key("name")]
    public string? RoleName { get; set; }
}

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
        var entry = _db.Users.Add(new UserEntity
        {
            Name = planned.Name,
            Email = planned.Email,
        });

        await _db.SaveChangesAsync();

        return new UserResource
        {
            Id = entry.Entity.Id,
            Email = entry.Entity.Email,
            Name = entry.Entity.Name
        };
    }

    public async Task DeleteAsync(UserResource resource)
    {
        await _db.Users.Where(x => x.Id == resource.Id).ExecuteDeleteAsync();
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
                    // TODO: create helper for this (AttributePath.For<T>(x => x.Name) or something)
                    new AttributePath(nameof(entity.Name).ToFirstLetterLower())
                }
            };
        }

        return new(new UserResource
        {
            // immutable
            Id = entity.Id,
            Name = entity.Name,

            // mutable
            Email = proposed.Email,
        });
    }

    public async Task<UserResource> ReadAsync(UserResource resource)
    {
        if (resource.Id == null)
        {
            return resource;
        }

        var entity = await _db.Users.FirstOrDefaultAsync(r => r.Id == resource.Id);
        if (entity == null)
        {
            return resource;
        }

        return new UserResource
        {
            Id = entity.Id,
            Email = entity.Email,
            Name = entity.Name
        };
    }

    public async Task<UserResource> UpdateAsync(UserResource? prior, UserResource planned)
    {
        var entity = await _db.Users.FirstOrDefaultAsync(r => r.Id == planned.Id)
            ?? throw new InvalidOperationException();

        entity.Name = planned.Name;
        entity.Email = planned.Email;

        await _db.SaveChangesAsync();

        return new UserResource
        {
            Id = entity.Id,
            Email = entity.Email,
            Name = entity.Name
        };
    }
}
