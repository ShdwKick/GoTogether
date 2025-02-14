using System.Text;
using System.Text.Json;
using GoTogether.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Data.Helpers;

namespace GoTogether.Services.RecoveryService;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly DatabaseConnection _databaseConnection;

    public EmailService(IHttpClientFactory httpClientFactory, DatabaseConnection databaseConnection)
    {
        _httpClient = httpClientFactory.CreateClient();
        _databaseConnection = databaseConnection;
    }

    public async Task<bool> SendRecoveryEmail(string address)
    {
        try
        {
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

            await SendMessage("SendRecoveryMail", Helpers.GenerateEmailCodeJson(address, code));

            return true;
        }
        catch (HttpRequestException hrq)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<bool> SendEmailConfirmationEmail()
    {
        try
        {
            var user = await Helpers.GetUserFromHeader();

            if (user == null)
                throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

            if (string.IsNullOrWhiteSpace(user.c_email))
                throw new ArgumentException("EMAIL_NOT_SELECTED_PROBLEM");

            int code = Helpers.GenerateCode();

            await _databaseConnection.ConfirmationCodes.AddAsync(new Codes()
            {
                c_email = user.c_email,
                n_code = code,
                id = Guid.NewGuid(),
                d_expiration_time = DateTime.UtcNow.AddMinutes(5),
            });

            await SendMessage("SendConfirmationMail", Helpers.GenerateEmailCodeJson(user.c_email, code));
            return true;
        }
        catch (HttpRequestException hrq)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    //TODO: переделать под rabbit
    public async Task SendMessage(string endpoint, StringContent body)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            throw new ArgumentException("INVALID_URL_PROBLEM");
        HttpResponseMessage response =
            await _httpClient.PostAsync($"{ConfigurationHelper.GetBaseUrl()}:7111/EmailSender/{endpoint}", body);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to send email. Status code: {response.StatusCode}");
    }

    public async Task<bool> SendInviteEmail(string email, Guid tripId)
    {
        try
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

            await SendMessage("SendInvite", Helpers.GenerateInviteMessageBodyJson(user.c_email, invite.c_code));
            return true;
        }
        catch (HttpRequestException hrq)
        {
            throw;
        }
    }
}