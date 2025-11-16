namespace TfNet.SampleEfProvider.Data;

internal class UserEntity
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public ICollection<UserRoleEntity> Roles { get; set; } = [];
}
