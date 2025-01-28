using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Data.Helpers;

namespace GraphQLServer.Services.RecoveryService;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _cache;
    private readonly DataBaseConnection _dataBaseConnection;

    public EmailService(IHttpContextAccessor httpContextAccessor, IMemoryCache cache,
        IHttpClientFactory httpClientFactory, DataBaseConnection dataBaseConnection)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
        _dataBaseConnection = dataBaseConnection;
    }

    public async Task<bool> SendRecoveryEmail(string address)
    {
        try
        {
            var user = await _dataBaseConnection.Users.FirstOrDefaultAsync(q => q.c_email == address);

            if (user == null)
                throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

            int code = Helpers.GenerateCode();

            await _dataBaseConnection.RecoveryCodes.AddAsync(new Codes()
            {
                c_email = address,
                n_code = code,
                id = Guid.NewGuid(),
                d_expiration_time = DateTime.UtcNow.AddMinutes(5),
            });

            string url = $"{ConfigurationHelper.GetBaseUrl()}:7111/EmailSender/SendRecoveryMail";

            await SendMessage(url, Helpers.GenerateEmailCodeJson(address, code));
            
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

    public async Task<bool> SendEmailConfirmationEMail()
    {
        try
        {
            var user = await Helpers.GetUserFromHeader(_dataBaseConnection, _httpContextAccessor);

            if (user == null)
                throw new ArgumentException("USER_NOT_FOUND_PROBLEM");
            
            if(string.IsNullOrWhiteSpace(user.c_email))
                throw new ArgumentException("EMAIL_NOT_SELECTED_PROBLEM");

            int code = Helpers.GenerateCode();

            await _dataBaseConnection.ConfirmationCodes.AddAsync(new Codes()
            {
                c_email = user.c_email,
                n_code = code,
                id = Guid.NewGuid(),
                d_expiration_time = DateTime.UtcNow.AddMinutes(5),
            });

            string url = $"{ConfigurationHelper.GetBaseUrl()}:7111/EmailSender/SendConfirmationMail";

            await SendMessage(url, Helpers.GenerateEmailCodeJson(user.c_email, code));
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

    public async Task SendMessage(string url, StringContent body)
    {
        if(string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("INVALID_URL_PROBLEM");
        HttpResponseMessage response = await _httpClient.PostAsync(url, body);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to send email. Status code: {response.StatusCode}");
    }

    public async Task<bool> SendInviteEmail(string email, Guid tripId)
    {
        try
        {
            var user = await Helpers.GetUserFromHeader(_dataBaseConnection, _httpContextAccessor);

            if (user == null)
                throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

            int code = Helpers.GenerateCode();


            var invite = new TripInvites()
            {
                id = Guid.NewGuid(),
                f_trip_id = tripId,
            };

            invite.c_code = Helpers.ComputeHash(invite.id.ToString());
            
            await _dataBaseConnection.TripInvites.AddAsync(invite);

            string url = $"{ConfigurationHelper.GetBaseUrl()}:7111/EmailSender/SendInvite";

            await SendMessage(url, Helpers.GenerateInviteJson(user.c_email, invite.c_code));
            return true;
        }
        catch (HttpRequestException hrq)
        {
            throw;
        }
    }
}