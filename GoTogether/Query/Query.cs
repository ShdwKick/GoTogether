using GoTogether.Services.RoleService;
using GoTogether.Services.TripService;
using HotChocolate.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Services;

namespace GoTogether
{
    public class Query
    {
        private readonly IUserService _userService;
        private readonly ITripService _tripService;
        private readonly IRoleService _roleService;

        public Query(IUserService userService, ITripService tripService, IRoleService roleService)
        {
            _userService = userService;
            _tripService = tripService;
            _roleService = roleService;
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
        [GraphQLDescription("AUTHORIZE-Получить данные о поездке по его id")]
        public async Task<Trip> GetTripInfo(Guid tripId)
        {
            var trip = await _tripService.GetTripInfo(tripId);

            if (trip == null)
                throw new ArgumentException("TRIP_NOT_FOUND_PROBLEM");

            return trip;
        }

        [Authorize]
        [UsePaging(MaxPageSize = 10,IncludeTotalCount = true)]
        [GraphQLDescription("AUTHORIZE-Получить данные о всех поездках человека по его id")]
        public async Task<List<Trip>> GetUserTrips(Guid userId)
        {
            var trip = await _tripService.GetUserTrips(userId);

            if (trip == null)
                throw new ArgumentException("TRIPS_NOT_FOUND_PROBLEM");

            return trip;
        }


        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получить данные о пользователе по его токену авторизации")]
        public async Task<User> GetUserByToken()
        {
            return await _userService?.GetUserByToken();
        }

        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получить данные о пользователе по его id")]
        public async Task<User> GetUserById(Guid userId)
        {
            return await _userService?.GetUserById(userId);
        }

        [GraphQLDescription("AUTHORIZE- Получить список пользовательских ролей")]
        public async Task<List<Role>> GetUserRoles()
        {
            return await _roleService.GetUserRoles();
        }
        
        [Authorize]
        [GraphQLDescription("AUTHORIZE- Получить список пользовательских ролей в рамках поездке")]
        public async Task<List<TripRole>> GetTripRoles()
        {
            return await _roleService.GetTripRoles();
        }
    }
}