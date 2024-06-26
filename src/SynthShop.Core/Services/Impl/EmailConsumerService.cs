using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using SynthShop.Domain.Models;
using SynthShop.Domain.Settings;

namespace SynthShop.Core.Services.Impl;

public class EmailConsumerService : BackgroundService
{
    private readonly RabbitMQSettings _rabbitmqSettings;
    private readonly EmailService _emailService;
    private readonly ILogger _logger;
    private IConnection _connection;
    private IModel _channel;

    public EmailConsumerService(IOptions<RabbitMQSettings> rabbitMQOptions, EmailService emailService, ILogger logger)
    {
        _rabbitmqSettings = rabbitMQOptions.Value;
        _emailService = emailService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //TODO use IOptions to retrieve RabbitMQ Configuration 

        var factory = new ConnectionFactory
        {
            HostName = _rabbitmqSettings.Host,
            UserName = _rabbitmqSettings.UserName,
            Password = _rabbitmqSettings.Password
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