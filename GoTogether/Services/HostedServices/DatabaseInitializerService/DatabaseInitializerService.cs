using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace GoTogether.Services.DatabaseInitializerService;

public class DatabaseInitializerService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseConnection>();
        await InitializeDataBase(dbContext);
    }

    public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;
    
    
    private async Task InitializeDataBase(DatabaseConnection databaseConnection)
    {
        await InitializeRoles(databaseConnection);
        await InitializeTripRoles(databaseConnection);
    }

    private async Task InitializeRoles(DatabaseConnection databaseConnection)
    {
        bool isDataAdded = false;
        if (await databaseConnection.UserRoles.FirstOrDefaultAsync(q => q.c_dev_name == "Admin") == null)
        {
            databaseConnection.UserRoles.Add(
                new UserRole
                {
                    id = Guid.NewGuid(),
                    c_name = "Admin",
                    c_dev_name = "Admin",
                    c_description = "Царь и бог",
                }
            );
            isDataAdded = true;
        }
        if (await databaseConnection.UserRoles.FirstOrDefaultAsync(q => q.c_dev_name == "User") == null)
        {
            databaseConnection.UserRoles.Add(
                new UserRole
                {
                    id = Guid.NewGuid(),
                    c_name = "User",
                    c_dev_name = "User",
                    c_description = "Обычный пользователь",
                }
            );
            isDataAdded = true;
        }

        if (isDataAdded)
            await databaseConnection.SaveChangesAsync();
    }
    
    private async Task InitializeTripRoles(DatabaseConnection databaseConnection)
    {
        bool isDataAdded = false;
        if (await databaseConnection.TripRoles.FirstOrDefaultAsync(q => q.c_dev_name == "Admin") == null)
        {
            databaseConnection.TripRoles.Add(
                new TripRole
                {
                    id = Guid.NewGuid(),
                    c_name = "Admin",
                    c_dev_name = "Admin",
                    c_description = "Царь и бог",
                    b_is_can_delete = true,
                    b_is_can_edit = true,
                    b_is_can_invite = true,
                    b_is_can_banish = true,
                }
            );
            isDataAdded = true;
        }
        if (await databaseConnection.TripRoles.FirstOrDefaultAsync(q => q.c_dev_name == "User") == null)
        {
            databaseConnection.TripRoles.Add(
                new TripRole
                {
                    id = Guid.NewGuid(),
                    c_name = "User",
                    c_dev_name = "User",
                    c_description = "Обычный пользователь",
                    b_is_can_edit = false,
                    b_is_can_invite = false,
                    b_is_can_banish = false,
                }
            );
            isDataAdded = true;
        }

        if (isDataAdded)
            await databaseConnection.SaveChangesAsync();
    }
}