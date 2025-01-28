using GraphQLServer.Services.PlaceService;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Services;

namespace GraphQLServer.Services.TripService;

public class TripService : ITripService
{
    private readonly IUserService _userService;
    private readonly IPlaceService _placeService;
    private readonly DataBaseConnection _dataBaseConnection;
    
    public TripService(IUserService userService, IPlaceService placeService, DataBaseConnection dataBaseConnection)
    {
        _userService = userService;
        _placeService = placeService;
        _dataBaseConnection = dataBaseConnection;
    }

    public async Task<Trip> GetTripInfo(Guid tripId)
    {
        return await _dataBaseConnection.Trips.FindAsync(tripId);
    }

    public async Task<List<Trip>> GetUserTrips(Guid userId)
    {
        if(userId == Guid.Empty)
            throw new ArgumentNullException(nameof(userId));

        return await _dataBaseConnection.Trips.Where(q=>q.f_author == userId).ToListAsync();
    }

    public Task<string> GenerateTripInviteCode(Guid tripId)
    {
        throw new NotImplementedException();
    }
}