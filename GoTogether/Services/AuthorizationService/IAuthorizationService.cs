using Server.Data;

namespace GoTogether.Services.AuthorizationService;

public interface IAuthorizationService
{
    Task<AuthorizationToken> GenerateNewTokenForUser(UserData user);
}