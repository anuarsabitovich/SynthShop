using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SynthShop.Domain.Models;
using SynthShop.Domain.Settings;

namespace SynthShop.Core.Services.Impl;

public class EmailProducer
{
    private readonly RabbitMQSettings _rabbitmqSettings;
    public EmailProducer(IOptions<RabbitMQSettings> rabbitMQOptions)
    {
        _rabbitmqSettings = rabbitMQOptions.Value;
    }

    public void SendMessage(SendEmailMessage sendEmailMessage)
    {
        var factory = new ConnectionFactory() { HostName = _rabbitmqSettings.Host, UserName = _rabbitmqSettings.UserName, Password = _rabbitmqSettings.Password };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sendEmailMessage));
        channel.BasicPublish(exchange: "", routingKey: "emailQueue", basicProperties: null, body: body);
    }
}