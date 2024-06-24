using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SynthShop.Domain.Models;

public class EmailProducer
{
    private readonly IConfiguration _configuration;
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;

    public EmailProducer(IConfiguration configuration)
    {
        _configuration = configuration;
        _hostName = _configuration["RabbitMQ:Host"];
        _userName = _configuration["RabbitMQ:UserName"];
        _password = _configuration["RabbitMQ:Password"];
    }

    public void SendMessage(SendEmailMessage sendEmailMessage)
    {
        var factory = new ConnectionFactory() { HostName = _hostName, UserName = _userName, Password = _password };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "emailQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sendEmailMessage));
        channel.BasicPublish(exchange: "", routingKey: "emailQueue", basicProperties: null, body: body);
    }
}