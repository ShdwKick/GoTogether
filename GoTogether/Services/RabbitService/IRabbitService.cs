namespace GoTogether.Services.RabbitService;

public interface IRabbitService
{
    Task InitializeServiceAsync();
    Task PublishMessageAsync(string messageType, object message);
}