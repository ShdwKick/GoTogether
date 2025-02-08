using Server.Data;

namespace GoTogether.Repositories;

public interface ICityRepository
{
    Task<bool> ExistsAsync(Guid cityId);
  
    Task<City?> GetByIdAsync(Guid cityId);  
}