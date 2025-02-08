using Server.Data;

namespace GoTogether.Repositories;

public class CityRepository : ICityRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public CityRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<bool> ExistsAsync(Guid cityId)
    {
        return await _databaseConnection.Cities.FindAsync(cityId) == null;
    }

    public async Task<City?> GetByIdAsync(Guid cityId)
    {
        return await _databaseConnection.Cities.FindAsync(cityId);
    }
}