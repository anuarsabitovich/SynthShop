using SynthShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IEmailProducer
    {
        void SendMessage(SendEmailMessage sendEmailMessage);
    }
}
