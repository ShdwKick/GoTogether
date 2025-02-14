using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace GoTogether.Controlers;

[ApiController]
[Route("api/[controller]")]
public class InviteController : ControllerBase
{
    private readonly DatabaseConnection _databaseConnection;

    public InviteController(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }


    [HttpGet("invite/{token}")]
    public async Task<Trip> ProcessInvite(string token)
    {
        //TODO: заменить на вызов метода из репозитория
        var invite = await _databaseConnection.TripInvites.FirstOrDefaultAsync(q => q.c_code == token);
        if(invite == null)
            throw new ArgumentException("INVALID_INVITE_CODE_PROBLEM");
        
        //TODO: заменить на вызов метода из репозитория
        var trip = await _databaseConnection.Trips.FirstOrDefaultAsync(q=>q.id == invite.f_trip_id);
        if(trip == null)
                throw new ArgumentException("TRIP_NOT_FOUND_PROBLEM");
        
        return trip;
    }


}