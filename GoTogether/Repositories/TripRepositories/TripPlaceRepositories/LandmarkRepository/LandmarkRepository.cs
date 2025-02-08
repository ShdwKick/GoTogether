using Server.Data;

namespace GoTogether.Repositories;

public class LandmarkRepository : ILandmarkRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public LandmarkRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<bool> ExistsAsync(Guid landmarkId)
    {
        return await _databaseConnection.Landmarks.FindAsync(landmarkId) == null;
    }
    public async Task<Landmark?> GetByIdAsync(Guid landmarkId)
    {
        return await _databaseConnection.Landmarks.FindAsync(landmarkId);
    }
}