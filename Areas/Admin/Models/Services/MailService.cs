using System.Net;
using System.Net.Mail;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Settings;
using Microsoft.Extensions.Options;

namespace GabriniCosmetics.Areas.Admin.Models.Services
{
    public class MailService: IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {   
            if(String.IsNullOrEmpty(mailRequest.ToEmail)) return;

            try
            {

                using MailMessage mail = new()
                {
                    From = new(_mailSettings.Mail),
                    Subject = mailRequest.Subject,
                    Body = mailRequest.Body,
                    IsBodyHtml = true
                };

                mail.To.Add(new(mailRequest.ToEmail));

                using SmtpClient smtp = new(_mailSettings.Host, _mailSettings.Port)
                {
                    UseDefaultCredentials = _mailSettings.UseDefaultCredentials,
                    EnableSsl = !_mailSettings.UseDefaultCredentials,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                if(!smtp.UseDefaultCredentials)
                {
                    smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
                }
                else
                {
                    smtp.Credentials = null;
                }

                await smtp.SendMailAsync(mail);

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }   
        }
    }
}