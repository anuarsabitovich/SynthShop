// Services/EmailConsumerService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SynthShop.Domain.Models;

public class EmailConsumerService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;

    public EmailConsumerService(IConfiguration configuration, EmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<SendEmailMessage>(Encoding.UTF8.GetString(body));
            await _emailService.SendEmailAsync(message);

        };
        channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}