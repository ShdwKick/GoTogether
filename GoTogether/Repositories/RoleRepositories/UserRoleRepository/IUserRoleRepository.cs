using Server.Data;

namespace GoTogether.Repositories.RoleRepositories.UserRoleRepository;

public interface IUserRoleRepository
{
    Task<UserRole?> GetRoleAsync(Guid roleId);
    Task<UserRole?> GetDefaultRoleAsync();
    Task<Guid> GetDefaultRoleGuidAsync();
    Task<bool> IsRoleExitsAsync(Guid roleId);
}