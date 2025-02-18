using System.Text;
using System.Text.Json;
using GoTogether.Data;
using GoTogether.Services.RabbitService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Data.Helpers;

namespace GoTogether.Services.RecoveryService;

//TODO: отрефакторить код
public class EmailService : IEmailService
{
    private readonly IRabbitService _rabbitService;
    private readonly DatabaseConnection _databaseConnection;

    public EmailService(IRabbitService rabbitService, DatabaseConnection databaseConnection)
    {
        _rabbitService = rabbitService;

        //TODO: удалить и заменить на вызов методов из репозитория
        _databaseConnection = databaseConnection;
    }

    public async Task<bool> SendRecoveryEmail(string address)
    {
        //TODO: удалить и заменить на вызов методов из репозитория
        var user = await _databaseConnection.Users.FirstOrDefaultAsync(q => q.c_email == address);

        if (user == null)
            throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

        int code = Helpers.GenerateCode();

        await _databaseConnection.RecoveryCodes.AddAsync(new Codes()
        {
            c_email = address,
            n_code = code,
            id = Guid.NewGuid(),
            d_expiration_time = DateTime.UtcNow.AddMinutes(5),
        });

        await SendMessage("Recovery", Helpers.GenerateEmailCodeJson(address, code));

        return true;
    }

    public async Task<bool> SendEmailConfirmationEmail()
    {
        var user = await Helpers.GetUserFromHeader();

        if (user == null)
            throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

        if (string.IsNullOrWhiteSpace(user.c_email))
            throw new ArgumentException("EMAIL_NOT_SELECTED_PROBLEM");

        int code = Helpers.GenerateCode();

        //TODO: удалить и заменить на вызов методов из репозитория
        await _databaseConnection.ConfirmationCodes.AddAsync(new Codes()
        {
            c_email = user.c_email,
            n_code = code,
            id = Guid.NewGuid(),
            d_expiration_time = DateTime.UtcNow.AddMinutes(5),
        });

        await SendMessage("Confirmation", Helpers.GenerateEmailCodeJson(user.c_email, code));
        return true;
    }


    public async Task<bool> SendInviteEmail(string email, Guid tripId)
    {
        var user = await Helpers.GetUserFromHeader();

        if (user == null)
            throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

        int code = Helpers.GenerateCode();


        var invite = new TripInvites()
        {
            id = Guid.NewGuid(),
            f_trip_id = tripId,
        };

        invite.c_code = Helpers.ComputeHash(invite.id.ToString());

        await _databaseConnection.TripInvites.AddAsync(invite);

        await SendMessage("Invite", Helpers.GenerateInviteMessageBodyJson(user.c_email, invite.c_code));
        return true;
    }

    public async Task SendMessage(string messageType, StringContent body)
    {
        await _rabbitService.PublishMessageAsync(messageType, body);
    }
}