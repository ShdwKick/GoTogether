using Server.Data;

namespace GoTogether.Services.RoleService;

public interface IRoleService
{
    Task<List<Role>> GetUserRoles();
    Task<List<TripRole>> GetTripRoles();
}