using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;

namespace GoTogether.Repositories.RoleRepositories.UserRoleRepository;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly DatabaseConnection _databaseConnection;
    private readonly IMemoryCache _memoryCache;

    public UserRoleRepository(DatabaseConnection databaseConnection, IMemoryCache memoryCache)
    {
        _databaseConnection = databaseConnection;
        _memoryCache = memoryCache;
    }

    public async Task<UserRole?> GetRoleAsync(Guid roleId)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException("INVALID_ROLE_ID_PROBLEM");

        if (_memoryCache.TryGetValue($"role{roleId}", out UserRole? role))
        {
            return role;
        }

        role = await _databaseConnection.UserRoles.FindAsync(roleId);
        _memoryCache.Set($"role{roleId}", role,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)));
        
        return role;
    }

    public async Task<UserRole?> GetDefaultRoleAsync()
    {
        return await _databaseConnection.UserRoles.FirstOrDefaultAsync(q => q.c_dev_name == "User");
    }

    public async Task<Guid> GetDefaultRoleGuidAsync()
    {
        var roleGuid = await GetDefaultRoleAsync();
        if(roleGuid == null)
            throw new ArgumentException("ROLE_NOT_FOUND_PROBLEM");
        return roleGuid.id;
    }

    public async Task<bool> IsRoleExitsAsync(Guid roleId)
    {
        return await _databaseConnection.UserRoles.Where(q=>q.id == roleId).AnyAsync();
    }
}