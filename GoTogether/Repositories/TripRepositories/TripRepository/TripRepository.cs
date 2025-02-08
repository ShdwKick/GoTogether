using GoTogether.Services.TripService;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace GoTogether.Repositories.TripRepositories;

public class TripRepository : ITripRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public TripRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }


    public async Task<Trip?> GetTripByIdAsync(Guid tripId)
    {
        return await _databaseConnection.Trips.FindAsync(tripId);
    }

    public async Task<List<Trip>> GetUserAuthorTripsAsync(Guid userId)
    {
        return await _databaseConnection.Trips.Where(q => q.f_author == userId).ToListAsync();
    }

    public async Task<List<Trip>> GetUserTripsAsync(Guid userId)
    {
        var userTrips = await _databaseConnection.UserTrips.Where(q => q.f_user_id == userId).ToListAsync();
        if (userTrips == null || userTrips.Count == 0)
            throw new ArgumentException("USER_DONT_HAVE_ANY_TRIPS_PROBLEM");

        var trips = new List<Trip>();
        trips.AddRange(await GetUserTripsAsync(userId));
        foreach (var userTrip in userTrips)
        {
            trips.Add(await GetTripByIdAsync(userTrip.f_trip_id));
        }

        return trips;
    }

    public async Task<Trip> CreateTripAsync(Trip trip)
    {
        if (trip == null || trip.id == Guid.Empty || trip.f_author == Guid.Empty || string.IsNullOrEmpty(trip.c_name))
            throw new ArgumentException("INVALID_TRIP_DATA_PROBLEM");

        _databaseConnection.Trips.Add(trip);
        await _databaseConnection.SaveChangesAsync();
        return trip;
    }

    public async Task AddTripCountryAsync(Guid tripId, Guid countryId, bool isNeedSaveDB = false)
    {
        if(tripId == Guid.Empty || countryId == Guid.Empty)
            throw new ArgumentException("ERROR_OCCURED_WHILE_ADDING_TRIP_COUNTRY_PROBLEM");
            
        _databaseConnection.TripCountries.Add(new TripCountrie()
        {
            id = Guid.NewGuid(),
            f_trip_id = tripId,
            f_country_id = countryId
        });
        if(isNeedSaveDB)
            await _databaseConnection.SaveChangesAsync();
    }

    public async Task AddTripCityAsync(Guid tripId, Guid cityId, bool isNeedSaveDB = false)
    {
        if(tripId == Guid.Empty || cityId == Guid.Empty)
            throw new ArgumentException("ERROR_OCCURED_WHILE_ADDING_TRIP_CITY_PROBLEM");
        
        _databaseConnection.TripCities.Add(new TripCity()
        {
            id = Guid.NewGuid(),
            f_trip_id = tripId,
            f_city_id = cityId
        });
        if(isNeedSaveDB)
            await _databaseConnection.SaveChangesAsync();
    }

    public async Task AddTripLandmarkAsync(Guid tripId, Guid landmarkId, bool isNeedSaveDB = false)
    {
        if(tripId == Guid.Empty || landmarkId == Guid.Empty)
            throw new ArgumentException("ERROR_OCCURED_WHILE_ADDING_TRIP_LANDMARK_PROBLEM");
        
        _databaseConnection.TripLandmarks.Add(new TripLandmark()
        {
            id = Guid.NewGuid(),
            f_trip_id = tripId,
            f_landmark_id = landmarkId
        });
        if(isNeedSaveDB)
            await _databaseConnection.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _databaseConnection.SaveChangesAsync();
    }
}