using Server.Data;

namespace GoTogether.Services.RecoveryService;

public interface IEmailService
{
    Task<bool> SendRecoveryEmail(string address);
    Task<bool> SendEmailConfirmationEMail();
    Task SendMessage(string url, StringContent body);
    Task<bool> SendInviteEmail(string email, Guid tripId);
}