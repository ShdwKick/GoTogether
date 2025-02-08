using Server.Data;

namespace GoTogether.Repositories;

public interface ILandmarkRepository
{
    Task<bool> ExistsAsync(Guid countryId);
    Task<Landmark?> GetByIdAsync(Guid countryId);
    
}