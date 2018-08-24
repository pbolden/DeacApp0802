using SendGrid.Helpers.Mail;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using SendGrid;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;

namespace DeaconCCGManagement.Services
{
    public class SendGridService
    {
        private readonly SendGridClient _client;

        // get SendGrid api key from the web.config file
        private string apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];

        private static readonly string MessageId = "X-Message-Id";

        public SendGridService()
        {
            _client = new SendGridClient(apiKey);
        }

     

        public EmailResponse Send(EmailContact email)
        {
            var sendGridMessage = new SendGridMessage
            {
                From = new EmailAddress(email.FromEmailAddress, email.SenderName),
                Subject = email.Subject,
                HtmlContent = email.HtmlBody
            };

            sendGridMessage.AddTo(new EmailAddress(email.ToEmailAddress));

            if (!string.IsNullOrWhiteSpace(email.BccEmailAddress))
            {
                sendGridMessage.AddBcc(new EmailAddress(email.BccEmailAddress));
            }

            if (!string.IsNullOrWhiteSpace(email.CcEmailAddress))
            {
                sendGridMessage.AddBcc(new EmailAddress(email.CcEmailAddress));
            }

            return ProcessResponse(_client.SendEmailAsync(sendGridMessage).Result);
        }

        public void SendPassAlongContact(ContactRecord record, UnitOfWork db)
        {
            // Get users in pastor or leadership role.  
            var users = db.AppUserRepository
                .FindUsers(u => db.AppUserRepository.IsInRole(u.Email, AppUserRole.Pastor)
                || db.AppUserRepository.IsInRole(u.Email, AppUserRole.DeaconLeadership));


            // Get user who is sending the email
            var appUser = db.AppUserRepository.FindUserById(record.AppUserId);

            var sb = new StringBuilder();
            foreach (var user in users)
            {
                var email = GetPassAlongEmail(record, user, db, sb);

                try
                {
                    // TODO: do something with response
                    // We could give the deacon confirmation email sent
                    var response = Send(email);

                    // Update 'pass along' record
                    var passAlongContact = db.PassAlongContactRepository.FindById(record.Id);
                    passAlongContact.PassAlongEmailSent = true;
                    //db.PassAlongContactRepository.Update(passAlongContact);
                    //db.Commit();
                }
                catch (Exception ex)
                {
                    throw new Exception("Pass along email not sent.");
                }


            }

        }

        private EmailContact GetPassAlongEmail(ContactRecord record,
            CCGAppUser user, UnitOfWork db, StringBuilder sb)
        {
            //
            // ***TEST ONLY***
            // I've replace the user's email address with
            // literal string for testing only
            //
            string toEmailAddress = "pbolden@zionmbc.org";
            //string toEmailAddress = pastor.Email;
            //

            var email = new EmailContact();
            email.SenderName = user.FullName;
            email.FromEmailAddress = user.Email;
            email.ToEmailAddress = toEmailAddress;
            email.Subject = $"From Deacon {user.FullName}";

            // For testing, the email to the pastor is formatted
            // like this:

            // Subject: From Deacon Paul Bolden
            // Body:
            // [Pass along message from deacon]
            // [Contact record subject]
            // [Contact record comments if not private]


            sb.Append(record.PassAlongComments);
            sb.Append(Environment.NewLine + Environment.NewLine);
            sb.Append(record.Subject);
            sb.Append(Environment.NewLine + Environment.NewLine);
            if (!record.Private)
                sb.Append(record.Comments);

            // record.PassAlongComments;

            // What do we want to email message to the pastor to say?
            // Should the subject be the same as the contact record
            // or should it be something like "From Deacon Paul"?

            sb.Clear();

            return email;
        }

        private EmailResponse ProcessResponse(Response response)
        {
            if (response.StatusCode.Equals(System.Net.HttpStatusCode.Accepted)
                || response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
            {
                return ToMailResponse(response);
            }

            var errorResponse = response.Body.ReadAsStringAsync().Result;
            throw new EmailServiceException(response.StatusCode.ToString(), errorResponse);
        }

        private static EmailResponse ToMailResponse(Response response)
        {
            if (response == null) return null;

            var headers = (HttpHeaders)response.Headers;
            var messageId = headers.GetValues(MessageId).FirstOrDefault();
            return new EmailResponse
            {
                UniqueMessageId = messageId,
                DateSent = DateTime.UtcNow,
            };
        }
    }
}