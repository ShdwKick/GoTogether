using GoTogether.Services.RabbitService;

namespace GoTogether.Services.RabbitHostedService;

public class RabbitHostedService : IHostedService
{
    private readonly IRabbitService _rabbitService;

    public RabbitHostedService(IRabbitService rabbitService)
    {
        _rabbitService = rabbitService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _rabbitService.InitializeServiceAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)=> Task.CompletedTask;
}