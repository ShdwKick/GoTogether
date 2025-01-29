using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Server.Data;

public class DatabaseConnection : DbContext
{
    public DbSet<UserData> Users { get; set; }
    public DbSet<AuthorizationToken> Authorization { get; set; }
    public DbSet<Codes> RecoveryCodes { get; set; }
    public DbSet<Codes> ConfirmationCodes { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<TripRole> TripRoles { get; set; }

    public DbSet<Message> Message { get; set; }
    public DbSet<Message> PrivateMessage { get; set; }
    public DbSet<ChatsFilterWords> ChatsFilterWords { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<UserTrips> UserTrips { get; set; }
    public DbSet<TripInvites> TripInvites { get; set; }


    private readonly string _connectionString;

    public DatabaseConnection(IConfiguration config)
    {
        _connectionString = config.GetSection("AppSettings:DefaultConnection").Value;
        //подключение к бд

#if DEBUG
        Database.EnsureDeleted();
#endif
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //строка подключения к бд
        optionsBuilder.UseNpgsql(_connectionString);
    }
}