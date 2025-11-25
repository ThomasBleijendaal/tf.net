namespace TfNet.SampleEfProvider.Data;

internal class UserRoleEntity
{
    public int Id { get; set; }

    public string? RoleName { get; set; }

    public ICollection<UserEntity> Users { get; set; } = [];
}
