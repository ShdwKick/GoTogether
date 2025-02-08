using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Server.Data;

public class DatabaseConnection : DbContext
{
    public DbSet<UserData> Users { get; set; }
    public DbSet<AuthorizationToken> AuthorizationTokens { get; set; }
    public DbSet<Codes> RecoveryCodes { get; set; }
    public DbSet<Codes> ConfirmationCodes { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<TripRole> TripRoles { get; set; }
    
    public DbSet<Trip> Trips { get; set; }
    public DbSet<UserTrips> UserTrips { get; set; }
    public DbSet<TripInvites> TripInvites { get; set; }
    public DbSet<TripCountrie> TripCountries { get; set; }
    public DbSet<TripCity> TripCities { get; set; }
    public DbSet<TripLandmark> TripLandmarks { get; set; }
    
    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Landmark> Landmarks { get; set; }


    // public DbSet<Message> Message { get; set; }
    // public DbSet<Message> PrivateMessage { get; set; }
    // public DbSet<ChatsFilterWords> ChatsFilterWords { get; set; }
    

    private readonly string _connectionString;

    public DatabaseConnection(IConfiguration config)
    {
        _connectionString = config.GetSection("AppSettings:DefaultConnection").Value;
        //подключение к бд

        Database.Migrate();
        //Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //строка подключения к бд
        optionsBuilder.UseNpgsql(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Codes>().ToTable("RecoveryCodes");
        modelBuilder.Entity<Codes>().ToTable("ConfirmationCodes");

        // Связь UserData → Role (один ко многим)
        modelBuilder.Entity<UserData>()
            .HasOne<UserRole>()
            .WithMany()
            .HasForeignKey(u => u.f_role)
            .OnDelete(DeleteBehavior.Restrict); // Защита от каскадного удаления

        // Связь UserData → AuthorizationToken (один к одному)
        modelBuilder.Entity<UserData>()
            .HasOne<AuthorizationToken>()
            .WithOne()
            .HasForeignKey<UserData>(u => u.f_authorization_token)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь Trip → UserData (автор поездки)
        modelBuilder.Entity<Trip>()
            .HasOne<UserData>()
            .WithMany()
            .HasForeignKey(t => t.f_author)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь UserTrips → UserData (пользователь поездки)
        modelBuilder.Entity<UserTrips>()
            .HasOne<UserData>()
            .WithMany()
            .HasForeignKey(ut => ut.f_user_id)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь UserTrips → Trip (поездка)
        modelBuilder.Entity<UserTrips>()
            .HasOne<Trip>()
            .WithMany()
            .HasForeignKey(ut => ut.f_trip_id)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь UserTrips → TripRole (роль в поездке)
        modelBuilder.Entity<UserTrips>()
            .HasOne<TripRole>()
            .WithMany()
            .HasForeignKey(ut => ut.f_user_trip_role)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь TripInvites → Trip (приглашение относится к поездке)
        modelBuilder.Entity<TripInvites>()
            .HasOne<Trip>()
            .WithMany()
            .HasForeignKey(ti => ti.f_trip_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}