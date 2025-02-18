using System.Text;
using System.Text.Json;
using GoTogether.Data;
using RabbitMQ.Client;

namespace GoTogether.Services.RabbitService;

public class RabbitService : IRabbitService
{
    private IConnectionFactory _factory;
    private IConnection _connection;
    private IChannel _channel;

    private readonly string _exchangeName;
    private readonly string _recoveryRoutingKeyName;
    private readonly string _confirmationRoutingKeyName;

    public RabbitService()
    {
        _factory = new ConnectionFactory()
        {
            HostName = ConfigurationHelper.GetRabbitHostName(),
            UserName = ConfigurationHelper.GetRabbitUserName(),
            Password = ConfigurationHelper.GetRabbitPassword(),
        };
        _exchangeName = "emailExchange";
        _recoveryRoutingKeyName = "email.recovery";
        _confirmationRoutingKeyName = "email.confirmation";
    }

    public async Task InitializeServiceAsync()
    {
        _connection = await _factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Direct);
        await _channel.QueueDeclareAsync(queue: _recoveryRoutingKeyName, durable: true, exclusive: false,
            autoDelete: false);
        await _channel.QueueDeclareAsync(queue: _confirmationRoutingKeyName, durable: true, exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(_recoveryRoutingKeyName, _exchangeName, _recoveryRoutingKeyName);
        await _channel.QueueBindAsync(_confirmationRoutingKeyName, _exchangeName, _confirmationRoutingKeyName);
    }

    public async Task PublishMessageAsync(string messageType, object message)
    {
        var routingKey = messageType switch
        {
            "Confirmation" => _confirmationRoutingKeyName,
            "Recovery" => _recoveryRoutingKeyName,
            _ => throw new ArgumentException("Unknown message type")
        };

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            Type = messageType,
            Payload = message
        }));

        await _channel.BasicPublishAsync(_exchangeName, routingKey, false, props, body);
    }
}