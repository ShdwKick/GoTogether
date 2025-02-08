using Server.Data;

namespace GoTogether.Services.RoleService;

public interface IRoleService
{
    Task<List<UserRole>> GetUserRoles();
    Task<List<TripRole>> GetTripRoles();
}