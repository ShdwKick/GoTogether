using Server.Data;

namespace GoTogether.Repositories.UserRepository;

public interface IUserRepository
{
    Task<User?> GetUserAsync(Guid userId);
    Task<UserData?> GetUserDataAsync(Guid userId);
    Task<bool> UserExistByNameAsync(string name);
    Task<bool> UserExistByEmailAsync(string email);
    Task<string?> CreateUserAsync(UserForCreate user, Guid? roleId);
    Task<string> LoginUser(string login, string password);
}