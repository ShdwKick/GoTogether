using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Helpers;

namespace GoTogether.Services.AuthorizationService;

public class AuthorizationService : IAuthorizationService
{
    private readonly DatabaseConnection _databaseConnection;

    public AuthorizationService(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<AuthorizationToken> GenerateNewTokenForUser(UserData user)
    {
        //TODO: переделать передачу ролей, брать из бд только если в её нет в клэймах и если роль изменилась
        //var userRole = await _userRoleRepository.GetRoleAsync(user.f_role);
        var token = new JwtSecurityTokenHandler().WriteToken(Helpers.GenerateNewToken(user.id.ToString(),
            "User"));

        var authorizationToken =
            await _databaseConnection.AuthorizationTokens.FirstOrDefaultAsync(q => q.id == user.f_authorization_token);
        if (authorizationToken == null)
            throw new ArgumentException("TOKEN_GENERATION_PROBLEM");

        authorizationToken.c_token = token;
        authorizationToken.c_hash = Helpers.ComputeHash(authorizationToken.c_token);
        await _databaseConnection.SaveChangesAsync();

        return authorizationToken;
    }
}