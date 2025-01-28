using GraphQLServer.Services.TripService;
using HotChocolate.Authorization;
using Server.Data;
using Server.Services;

namespace GraphQLServer
{

    public class Query
    {
        private readonly IUserService _userService;
        private readonly ITripService _tripService;

        public Query(IUserService userService, ITripService tripService)
        {
            _userService = userService;
            _tripService = tripService;
        }


        [GraphQLDescription("Получить серверное время")]
        public DateTime GetServerCurrentDateTime()
        {
            return DateTime.Now;
        }
        
        [GraphQLDescription("Получить серверное время по UTC")]
        public DateTime GetServerCurrentUTCDateTime()
        {
            return DateTime.UtcNow;
        }
        
        [Authorize]
        [GraphQLDescription("Получить данные о поездке по его id")]
        public async Task<Trip> GetTripInfo(Guid tripId)
        {
            var trip = await _tripService.GetTripInfo(tripId);
        
            if (trip == null)
                throw new ArgumentException("TRIP_NOT_FOUND_PROBLEM");
            
            return trip;
        }
        
        [Authorize]
        [GraphQLDescription("Получить данные о всех поездках человека по его id")]
        public async Task<List<Trip>> GetUserTrips(Guid userId)
        {
            var trip = await _tripService.GetUserTrips(userId);
        
            if (trip == null)
                throw new ArgumentException("TRIP_NOT_FOUND_PROBLEM");
            
            return trip;
        }
        
        
        [Authorize]
        [GraphQLDescription("Получить данные о пользователе по его токену авторизации")]
        public async Task<User> GetUserByToken()
        {
            return await _userService?.GetUserByToken();
        }
        
        [Authorize]
        [GraphQLDescription("Получить данные о пользователе по его id")]
        public async Task<User> GetUserById(Guid userId)
        {
            return await _userService?.GetUserById(userId);
        }
    }
}