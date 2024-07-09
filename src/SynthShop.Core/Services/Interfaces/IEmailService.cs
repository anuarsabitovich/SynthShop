using SynthShop.Domain.Models;

namespace SynthShop.Core.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(SendEmailMessage sendEmailMessage);
}