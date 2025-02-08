using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace GoTogether.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public CountryRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<bool> ExistsAsync(Guid countryId)
    {
        return await _databaseConnection.Countries.FindAsync(countryId) == null;
    }

    public async Task<Country?> GetByIdAsync(Guid countryId)
    {
        return await _databaseConnection.Countries.FindAsync(countryId);
    }
}