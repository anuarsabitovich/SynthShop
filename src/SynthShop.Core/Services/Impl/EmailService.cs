using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Models;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IAmazonSimpleEmailService amazonSimpleEmailService, IConfiguration configuration)
    {
        _logger = logger;
        _amazonSimpleEmailService = amazonSimpleEmailService;
        _configuration = configuration;
    }

    public async Task  SendEmailAsync(SendEmailMessage sendEmailMessage)
    {
        try
        {
            var sendRequest = new SendEmailRequest
            {
                Source = _configuration["SES:SenderAddress"],
                Destination = new Destination
                {
                    ToAddresses = new List<string> { sendEmailMessage.to.Trim() }
                },
                Message = new Message
                {
                    Subject = new Content(sendEmailMessage.subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = sendEmailMessage.body
                        },
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = sendEmailMessage.body
                        }
                    }
                }
            };

            var response = await _amazonSimpleEmailService.SendEmailAsync(sendRequest);
            _logger.LogInformation("Email sent successfully with MessageId: {MessageId}", response.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError("SendEmailAsync failed with exception: {ExceptionMessage}", ex.Message);
        }


    }
}