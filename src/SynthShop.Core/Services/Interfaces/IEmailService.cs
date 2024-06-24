using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthShop.Domain.Models;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(SendEmailMessage sendEmailMessage);
    }
}
