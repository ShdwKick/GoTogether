using GoTogether.Repositories.RoleRepositories.UserRoleRepository;
using GoTogether.Services.AuthorizationService;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Helpers;

namespace GoTogether.Repositories.UserRepository;

public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _databaseConnection;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuthorizationService _authorizationService;

    public UserRepository(DatabaseConnection databaseConnection, IUserRoleRepository userRoleRepository,
        IAuthorizationService authorizationService)
    {
        _databaseConnection = databaseConnection;
        _userRoleRepository = userRoleRepository;
        _authorizationService = authorizationService;
    }

    public async Task<User?> GetUserAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("INVALID_USER_ID_PROBLEM");

        var user = await _databaseConnection.Users.FindAsync(userId);

        return new User()
        {
            Id = user.id,
            Nickname = user.c_nickname,
            Email = user.c_email,
            IsEmailConfirmed = user.b_is_mail_confirmed,
            RegistrationDate = user.d_registration_date,
            Role = await _userRoleRepository.GetRoleAsync(userId),
        };
    }

    public async Task<UserData?> GetUserDataAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("INVALID_USER_ID_PROBLEM");

        return await _databaseConnection.Users.FindAsync(userId);
    }

    public async Task<bool> UserExistByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("INVALID_USER_NAME_PROBLEM");

        return await _databaseConnection.Users.Where(q => q.c_nickname == name).AnyAsync();
    }

    public async Task<bool> UserExistByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("INVALID_USER_EMAIL_PROBLEM");

        return await _databaseConnection.Users.Where(q => q.c_email == email).AnyAsync();
    }


    public async Task<string?> CreateUserAsync(UserForCreate user, Guid? roleId)
    {
        var usr = new UserData
        {
            id = Guid.NewGuid(),
            c_nickname = user.Nickname,
            c_email = user.Email,
            c_password = Helpers.ComputeHash(user.Password),
            d_registration_date = DateTime.UtcNow,
            b_is_mail_confirmed = false,
        };

        if (roleId == null || roleId == Guid.Empty)
            usr.f_role = await _userRoleRepository.GetDefaultRoleGuidAsync();
        else if (await _userRoleRepository.IsRoleExitsAsync((Guid)roleId))
            usr.f_role = (Guid)roleId;
        else
            throw new ArgumentException("ROLE_NOT_FOUND_PROBLEM");

        var newToken = await _authorizationService.GenerateNewTokenForUser(usr);
        usr.f_authorization_token = newToken.id;
        

        await _databaseConnection.Users.AddAsync(usr);
        await _databaseConnection.SaveChangesAsync();

        return newToken.c_token;
    }

    public async Task<string> LoginUser(string login, string password)
    {
        string passwordHash = Helpers.ComputeHash(password);

        var user = await _databaseConnection.Users.FirstOrDefaultAsync(q =>
            q.c_email == login && q.c_password == passwordHash);
        if (user == null)
            throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

        var token = await _authorizationService.GenerateNewTokenForUser(user);
        return token.c_token;
    }
}