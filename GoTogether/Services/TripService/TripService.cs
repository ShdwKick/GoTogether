using GoTogether.Repositories;
using GoTogether.Repositories.TripRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Data.Helpers;
using Server.Services;

namespace GoTogether.Services.TripService;

public class TripService : ITripService
{
    private readonly IUserService _userService;
    private readonly ITripRepository _tripRepository;
    private readonly DatabaseConnection _databaseConnection;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _memoryCache;
    private readonly ICountryRepository _countryRepository;
    private readonly ICityRepository _cityRepository;
    private readonly ILandmarkRepository _landmarkRepository;

    public TripService(IUserService userService, DatabaseConnection databaseConnection,
        IHttpContextAccessor httpContextAccessor, ITripRepository tripRepository, IMemoryCache memoryCache,
        ICountryRepository countryRepository, ICityRepository cityRepository, ILandmarkRepository landmarkRepository)
    {
        _userService = userService;
        _databaseConnection = databaseConnection;
        _httpContextAccessor = httpContextAccessor;
        _tripRepository = tripRepository;
        _memoryCache = memoryCache;
        _countryRepository = countryRepository;
        _cityRepository = cityRepository;
        _landmarkRepository = landmarkRepository;
    }

    public async Task<Trip> GetTripInfo(Guid tripId)
    {
        if (tripId == Guid.Empty)
            throw new ArgumentException("INVALID_TRIP_ID_PROBLEM");
        if (_memoryCache.TryGetValue($"trip{tripId}", out Trip trip))
        {
            return trip;
        }

        trip = await _tripRepository.GetTripByIdAsync(tripId);
        if (trip == null)
            throw new ArgumentException("TRIP_NOT_FOUND_PROBLEM");
        _memoryCache.Set($"trip{tripId}", trip,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
        return trip;
    }

    public async Task<List<Trip>> GetUserTrips(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("INVALID_USER_ID_PROBLEM");

        if (_memoryCache.TryGetValue($"userTrips{userId}", out List<Trip> trips))
        {
            return trips;
        }

        trips = await _tripRepository.GetUserTripsAsync(userId);
        if (trips == null)
            throw new ArgumentException("TRIP_NOT_FOUND_PROBLEM");
        _memoryCache.Set($"userTrips{userId}", trips,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
        return trips;
    }

    public Task<string> GenerateTripInviteCode(Guid tripId)
    {
        throw new NotImplementedException();
    }

    public async Task<Trip> CreateTrip(TripForCreate trip)
    {
        var author = await Helpers.GetUserFromHeader(_databaseConnection, _httpContextAccessor);
        if (author == null)
            throw new ArgumentException("USER_NOT_FOUND_PROBLEM");
        
        if(string.IsNullOrEmpty(trip.c_name))
            throw new ArgumentException("INVALID_TRIP_NAME_PROBLEM");
        if(trip.d_start_date > trip.d_end_date)
            throw new ArgumentException("INVALID_TRIP_DATES_PROBLEM");

        var newTrip = new Trip()
        {
            id = Guid.NewGuid(),
            c_name = trip.c_name,
            c_description = trip.c_description,
            d_start_date = trip.d_start_date,
            d_end_date = trip.d_end_date,
            f_author = author.id,
        };
        await _tripRepository.CreateTripAsync(newTrip);

        if (trip.f_countries != null && trip.f_countries.Count > 0)
        {
            foreach (var countryId in trip.f_countries)
            {
                if (!await _countryRepository.ExistsAsync(countryId))
                    throw new ArgumentException($"COUNTRY_{countryId}_NOT_FOUND_PROBLEM");
                await _tripRepository.AddTripCountryAsync(newTrip.id, countryId, false);
            }
        }

        if (trip.f_cities != null && trip.f_cities.Count > 0)
        {
            foreach (var cityId in trip.f_cities)
            {
                if (!await _cityRepository.ExistsAsync(cityId))
                    continue;
                if (!trip.f_countries.Contains(cityId))
                    throw new ArgumentException("CITY_IS_NOT_FROM_THIS_COUNTRIES_PROBLEM");
                await _tripRepository.AddTripCityAsync(newTrip.id, cityId, false);
            }
        }

        if (trip.f_landmarks != null && trip.f_landmarks.Count > 0)
        {
            foreach (var landmarkId in trip.f_landmarks)
            {
                var landmark = await _landmarkRepository.GetByIdAsync(landmarkId);
                if (landmark == null)
                    continue;
                if (!trip.f_countries.Contains(landmark.f_country_id))
                    throw new ArgumentException("LANDMARK_IS_NOT_FROM_THIS_COUNTRIES_PROBLEM");
                if (landmark.f_city_id != null && !trip.f_cities.Contains((Guid)landmark.f_city_id))
                    throw new ArgumentException("LANDMARK_IS_NOT_FROM_THIS_CITY_PROBLEM");
                await _tripRepository.AddTripLandmarkAsync(newTrip.id, landmarkId, false);
            }
        }

        await _tripRepository.SaveChangesAsync();
        return newTrip;
    }
}