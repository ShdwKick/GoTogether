using Server.Data;

namespace GoTogether.Repositories.TripRepositories;

public interface ITripRepository
{
    Task<Trip?> GetTripByIdAsync(Guid tripId);
    Task<List<Trip>> GetUserAuthorTripsAsync(Guid userId);
    Task<List<Trip>> GetUserTripsAsync(Guid userId);
    Task<Trip> CreateTripAsync(Trip trip);
    Task AddTripCountryAsync(Guid tripId, Guid countryId, bool isNeedSaveDB);
    Task AddTripCityAsync(Guid tripId, Guid cityId, bool isNeedSaveDB);
    Task AddTripLandmarkAsync(Guid tripId, Guid landmarkId, bool isNeedSaveDB);
    Task SaveChangesAsync();
}