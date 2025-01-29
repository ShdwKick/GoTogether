using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace GoTogether.Controlers;

[ApiController]
[Route("api/[controller]")]
public class InviteController : ControllerBase
{
    private readonly DataBaseConnection _dataBaseConnection;

    public InviteController(DataBaseConnection dataBaseConnection)
    {
        _dataBaseConnection = dataBaseConnection;
    }


    [HttpGet("invite/{token}")]
    public async Task<Trip> ProcessInvite(string token)
    {
        var invite = await _dataBaseConnection.TripInvites.FirstOrDefaultAsync(q => q.c_code == token);
        if(invite == null)
            throw new ArgumentException("INVALID_INVITE_CODE_PROBLEM");
        
        var trip = await _dataBaseConnection.Trips.FirstOrDefaultAsync(q=>q.id == invite.f_trip_id);
        if(trip == null)
                throw new ArgumentException("TRIP_NOT_FOUND_PROBLEM");
        
        return trip;
    }


}