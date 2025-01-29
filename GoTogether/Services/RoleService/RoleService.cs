using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;

namespace GoTogether.Services.RoleService;

public class RoleService : IRoleService
{
    private readonly IMemoryCache _memoryCache;
    private readonly DatabaseConnection _databaseConnection;

    public RoleService(DatabaseConnection databaseConnection, IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _databaseConnection = databaseConnection;
    }


    public async Task<List<Role>> GetUserRoles()
    {
        if (_memoryCache.TryGetValue("UserRoles", out List<Role> roles))
        {
            if (roles == null || roles.Any())
                return roles;

            roles = await _databaseConnection.Roles.ToListAsync();
            _memoryCache.Set("UserRoles", roles,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(2)));
            return roles;
        }
        else
        {
            roles = await _databaseConnection.Roles.ToListAsync();
            _memoryCache.Set("UserRoles", roles,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(2)));
            return roles;
        }
    }

    public async Task<List<TripRole>> GetTripRoles()
    {
        if (_memoryCache.TryGetValue("TripRoles", out List<TripRole> roles))
        {
            if (roles != null || roles.Any())
                return roles;

            roles = await _databaseConnection.TripRoles.ToListAsync();
            _memoryCache.Set("TripRoles", roles,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(2)));
            return roles;
        }
        else
        {
            roles = await _databaseConnection.TripRoles.ToListAsync();
            _memoryCache.Set("TripRoles", roles,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(2)));
            return roles;
        }
    }
}