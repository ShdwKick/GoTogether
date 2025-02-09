using GoTogether.Repositories.RoleRepositories.UserRoleRepository;
using GoTogether.Repositories.UserRepository;
using Server.Data;
using Server.Data.Helpers;

namespace Server.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public UserService(IUserRoleRepository userRoleRepository, IUserRepository userRepository)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
    }

    public async Task<User> GetUserByToken()
    {
        var userData = await Helpers.GetUserFromHeader();
        return await GetUserFromUserData(userData);
    }

    public async Task<User> GetUserById(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("INVALID_USER_ID_PROBLEM");

        var userData = await _userRepository.GetUserAsync(userId);
        if (userData == null)
            throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

        return userData;
    }

    public async Task<string> CreateUser(UserForCreate user, Guid? roleGuid = null)
    {
        if (await _userRepository.UserExistByEmailAsync(user.Email) ||
            await _userRepository.UserExistByNameAsync(user.Nickname))
        {
            throw new ArgumentException("EMAIL_OR_NAME_EXIST_PROBLEM");
        }

        return await _userRepository.CreateUserAsync(user, roleGuid);
    }

    public async Task<string> LoginUser(string login, string password)
    {
        if(string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("INVALID_LOGIN_OR_PASSWORD_PROBLEM");
        
        return await _userRepository.LoginUser(login, password);
    }

    public async Task<User> GetUserFromUserData(UserData userData)
    {
        return new User()
        {
            Id = userData.id,
            Nickname = userData.c_nickname,
            Email = userData.c_email,
            IsEmailConfirmed = userData.b_is_mail_confirmed,
            RegistrationDate = userData.d_registration_date,
            Role = await _userRoleRepository.GetRoleAsync(userData.f_role)
        };
    }
}