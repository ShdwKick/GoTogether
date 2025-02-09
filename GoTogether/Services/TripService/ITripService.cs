using Server.Data;

namespace GoTogether.Services.TripService;

public interface ITripService
{
    Task<Trip> GetTripInfo(Guid tripId);
    Task<FullTripData> GetFullTripInfo(Guid tripId);
    Task<List<Trip>> GetUserTrips(Guid userId);
    Task<string> GenerateTripInviteCode(Guid tripId);
    Task<Trip> CreateTrip(TripForCreate trip);
}