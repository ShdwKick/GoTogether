using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace GraphQLServer.Services.HostedServices.DatabaseInitializerService;

public class DataBaseInitializerService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DataBaseInitializerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataBaseConnection>();
        await InitializeDataBase(dbContext);
    }

    public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;
    
    
    private async Task InitializeDataBase(DataBaseConnection dataBaseConnection)
    {
        await InitializeRoles(dataBaseConnection);
        await InitializeTripRoles(dataBaseConnection);
    }

    private async Task InitializeRoles(DataBaseConnection dataBaseConnection)
    {
        bool isDataAdded = false;
        if (await dataBaseConnection.Roles.FirstOrDefaultAsync(q => q.c_dev_name == "Admin") == null)
        {
            dataBaseConnection.Roles.Add(
                new Role
                {
                    id = Guid.NewGuid(),
                    c_name = "Admin",
                    c_dev_name = "Admin",
                    c_description = "Царь и бог",
                }
            );
            isDataAdded = true;
        }
        if (await dataBaseConnection.Roles.FirstOrDefaultAsync(q => q.c_dev_name == "User") == null)
        {
            dataBaseConnection.Roles.Add(
                new Role
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
            await dataBaseConnection.SaveChangesAsync();
    }
    
    private async Task InitializeTripRoles(DataBaseConnection dataBaseConnection)
    {
        bool isDataAdded = false;
        if (await dataBaseConnection.TripRoles.FirstOrDefaultAsync(q => q.c_dev_name == "Admin") == null)
        {
            dataBaseConnection.TripRoles.Add(
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
        if (await dataBaseConnection.TripRoles.FirstOrDefaultAsync(q => q.c_dev_name == "User") == null)
        {
            dataBaseConnection.TripRoles.Add(
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
            await dataBaseConnection.SaveChangesAsync();
    }
}