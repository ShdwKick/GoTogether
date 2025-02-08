using Server.Data;

namespace GoTogether.Repositories;

public interface ICountryRepository
{
    Task<bool> ExistsAsync(Guid countryId);
    Task<Country?> GetByIdAsync(Guid countryId);
}