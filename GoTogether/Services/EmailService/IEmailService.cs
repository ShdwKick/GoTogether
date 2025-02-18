using Server.Data;

namespace GoTogether.Services.RecoveryService;

public interface IEmailService
{
    Task<bool> SendRecoveryEmail(string address);
    Task<bool> SendEmailConfirmationEmail();
    Task SendMessage(string messageType, StringContent body);
    Task<bool> SendInviteEmail(string email, Guid tripId);
}