using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GabriniCosmetics.Areas.Admin.Models.Interface
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}