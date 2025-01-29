using GoTogether.Services.PlaceService;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Services;

namespace GoTogether.Services.TripService;

public class TripService : ITripService
{
    private readonly IUserService _userService;
    private readonly IPlaceService _placeService;
    private readonly DatabaseConnection _databaseConnection;
    
    public TripService(IUserService userService, IPlaceService placeService, DatabaseConnection databaseConnection)
    {
        _userService = userService;
        _placeService = placeService;
        _databaseConnection = databaseConnection;
    }

    public async Task<Trip> GetTripInfo(Guid tripId)
    {
        return await _databaseConnection.Trips.FindAsync(tripId);
    }

    public async Task<List<Trip>> GetUserTrips(Guid userId)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException("INVALID_USER_GUID_PROBLEM");

        return await _databaseConnection.Trips.Where(q=>q.f_author == userId).ToListAsync();
    }

    public Task<string> GenerateTripInviteCode(Guid tripId)
    {
        throw new NotImplementedException();
    }
}