using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Models;
using SynthShop.Domain.Settings;

namespace SynthShop.Core.Services.Impl;

public class EmailProducer : IEmailProducer
{
    private readonly RabbitMQSettings _rabbitmqSettings;

    public EmailProducer(IOptions<RabbitMQSettings> rabbitMQOptions)
    {
        _rabbitmqSettings = rabbitMQOptions.Value;
    }

    public void SendMessage(SendEmailMessage sendEmailMessage)
    {
        var factory = RabbitMQExtension.GetFactory(_rabbitmqSettings.Host, _rabbitmqSettings.UserName,
            _rabbitmqSettings.Password);
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare("emailQueue", false, false, false, null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sendEmailMessage));
        channel.BasicPublish("", "emailQueue", null, body);
    }
}