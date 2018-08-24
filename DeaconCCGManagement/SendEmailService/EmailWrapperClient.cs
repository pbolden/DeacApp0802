using DeaconCCGManagement.ViewModels;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace DeaconCCGManagement.SendEmailService
{
    public class EmailWrapperClient : IEmailClient
    {
        private readonly SendGridClient _client;

        public EmailWrapperClient()
        {
            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            _client = new SendGridClient(apiKey);
        }
        /// <summary>
        /// Sends a single email asynchronously.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public async Task<Response> SendSingleEmailAsync(EmailContact email)
        {
            var from = new EmailAddress(email.FromEmailAddress, email.SenderName);
            var to = new EmailAddress(email.ToEmailAddress, email.ReceiverName);          

            var emailToSend = MailHelper.CreateSingleEmail(
                from,
                to,
                email.Subject,
                email.PlainTextBody,
                email.HtmlBody);


            return await _client.SendEmailAsync(emailToSend);
        }
        /// <summary>
        /// Sends one email to multiple recipients asynchronously.
        /// </summary>
        /// <param name="toAddressess">A list of EmailContacts holding 'to' email addresses and recipients names.</param>
        /// <param name="email">The email to send.</param>
        /// <returns></returns>
        public async Task<Response> SendMultipleEmailsAsync(List<EmailContact> toAddressess, EmailContact email)
        {
            var tos = new List<EmailAddress>();
            foreach (var to in toAddressess)            
                tos.Add(new EmailAddress(to.ToEmailAddress, to.ReceiverName));            

            var from = new EmailAddress(email.FromEmailAddress, email.SenderName);
            var emailsToSend = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, email.Subject, email.PlainTextBody, email.HtmlBody);
            return await _client.SendEmailAsync(emailsToSend);
        }
       
    }
}