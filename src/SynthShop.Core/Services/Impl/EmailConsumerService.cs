// Services/EmailConsumerService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SynthShop.Domain.Models;

public class EmailConsumerService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private readonly ILogger _logger;
    private IConnection _connection;
    private IModel _channel;


    public EmailConsumerService(IConfiguration configuration, EmailService emailService, ILogger logger)
    {
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        }; 
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<SendEmailMessage>(Encoding.UTF8.GetString(body));

            try
            {
                await _emailService.SendEmailAsync(message);
            }
            catch (Exception e)
            {
                _logger.Error(e, e.Message);
            } 
        };
        _channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);
        stoppingToken.Register(() =>
        {
            _channel?.Close();
            _connection?.Close();
        });
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}