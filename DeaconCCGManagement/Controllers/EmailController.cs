using System;
using System.Web.Mvc;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Net;
using DeaconCCGManagement.SendEmailService;
using System.Threading.Tasks;
using System.Collections.Generic;
using SendGrid;
using DeaconCCGManagement.Utilities;
using DeaconCCGManagement.PushNotifications;
using System.Web.Security.AntiXss;
using Elmah;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class EmailController : ControllerBase
    {

        public EmailController()
        {

        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult SendEmail(int memberId)
        {
            var emailContact = new EmailContact();

            var user = unitOfWork.AppUserRepository.FindUserById(User.Identity.GetUserId());
            var member = unitOfWork.MemberRepository.FindMemberById(memberId);

            emailContact.FromEmailAddress = user.Email;
            emailContact.ToEmailAddress = member.EmailAddress;
            emailContact.MemberId = memberId;
            emailContact.SenderName = user.FullName;
            emailContact.DateSent = DateTime.Now;
            emailContact.PlainTextBody = string.Empty;
            emailContact.ReceiverName = $"{member.FirstName} {member.LastName}";


            var emailViewModel = new EmailViewModel
            {
                EmailContact = emailContact,
                Bulk = false,
                IsTesting = bool.Parse(ConfigurationManager.AppSettings["TestSendGrid"])
            };

            emailViewModel.StatusNotification = new NotificationViewModel();

            return View(emailViewModel);
        }

        public ActionResult SendBulkEmail()
        {
            var emailViewModel = TempData["EmailViewModel"];

            if (emailViewModel == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("SendEmail", emailViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> SendEmail(EmailViewModel emailViewModel)
        {
            if (!ModelState.IsValid) return View(emailViewModel);

            // sanitize the html from user
            emailViewModel.EmailContact.HtmlBody =
                AntiXssEncoder.HtmlEncode(emailViewModel.EmailContact.HtmlBody, false);
            emailViewModel.EmailContact.HtmlBody = 
                WebUtility.HtmlDecode(emailViewModel.EmailContact.HtmlBody);



            if (emailViewModel == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var client = new EmailWrapperClient();

            if (emailViewModel.IsTesting && !string.IsNullOrEmpty(emailViewModel.TestToEmail))
            {
                emailViewModel.EmailContact.ToEmailAddress = emailViewModel.TestToEmail;
                emailViewModel.EmailContact.ReceiverName = "Deacon App Test SendGrid";
            }

            emailViewModel.EmailContact.PlainTextBody =
                HtmlRemoval.ConvertHtmlToPlainText(emailViewModel.EmailContact.HtmlBody);

            if (emailViewModel.Bulk)
                emailViewModel = await SendBulkEmailsAsync(emailViewModel, client);

            else // send email to one member 
                emailViewModel = await SendSingleEmailAsync(emailViewModel, client);

            return View("EmailComplete", emailViewModel);
        }

        private void StoreEmailContact(EmailContact emailContact)
        {
            var member = unitOfWork.MemberRepository.FindMemberById(emailContact.MemberId);

            // Get the contact type id for 'email'
            var contactType = unitOfWork.ContactTypeRepository
                .Find(ct => ct.Name.Equals("Email", StringComparison.CurrentCultureIgnoreCase) ||
                            ct.Name.Equals("E-mail", StringComparison.CurrentCultureIgnoreCase));

            // To assign user id to contact record property
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            var contact = new ContactRecord
            {
                CCGMemberId = member.Id,
                AppUserId = user.Id,
                ContactTypeId = contactType.Id,
                ContactDate = emailContact.DateSent,
                Subject = emailContact.Subject,
                Timestamp = DateTime.Now
            };
            unitOfWork.ContactRecordRepository.Add(contact);
        }

        private void StoreBulkEmailContacts(List<EmailContact> emailContacts)
        {
            foreach (var email in emailContacts)
            {
                StoreEmailContact(email);
            }
        }

        private async Task<EmailViewModel> SendSingleEmailAsync(EmailViewModel emailViewModel, EmailWrapperClient client)
        {

            Response response;
            try
            {
                response = await client.SendSingleEmailAsync(emailViewModel.EmailContact);

                if (IsResponseOk(response))
                {
                    emailViewModel.StatusNotification = GetStatusNotification(EmailStatus.EmailDelivered, emailViewModel.EmailContact.ReceiverName);
                    emailViewModel.EmailContact.DateSent = DateTime.Now;
                    StoreEmailContact(emailViewModel.EmailContact);
                }
                else
                    emailViewModel.StatusNotification = GetStatusNotification(EmailStatus.EmailNotDelivered, emailViewModel.EmailContact.ReceiverName);
                
            }
            catch (Exception ex)
            {
                // log caught exception with Elmah
                ErrorSignal.FromCurrentContext().Raise(ex);

                emailViewModel.StatusNotification = GetStatusNotification(EmailStatus.EmailNotDelivered, emailViewModel.EmailContact.ReceiverName);
            }

            if (emailViewModel.StatusNotification != null)
                emailViewModel.HasStatusNotification = true;
            return emailViewModel;


        }

        private async Task<EmailViewModel> SendBulkEmailsAsync(EmailViewModel emailViewModel, EmailWrapperClient client)
        {
            Response response;
            var bulkContacts = GetBulkEmailContacts(emailViewModel);

            try
            {
                response = await client.SendMultipleEmailsAsync(bulkContacts, emailViewModel.EmailContact);
                if (IsResponseOk(response))
                {
                    //GetStatusNotification(EmailStatus.BulkEmailsDelivered);
                    emailViewModel.StatusNotification = GetStatusNotification(EmailStatus.BulkEmailsDelivered);
                    emailViewModel.EmailContact.DateSent = DateTime.Now;
                    StoreBulkEmailContacts(bulkContacts);
                }
                else
                {
                    //GetStatusNotification(EmailStatus.BulkEmailsNotDelivered);
                    emailViewModel.StatusNotification = GetStatusNotification(EmailStatus.BulkEmailsNotDelivered);
                }
            }
            catch (Exception ex)
            {
                // log caught exception with Elmah
                ErrorSignal.FromCurrentContext().Raise(ex);

                emailViewModel.StatusNotification = GetStatusNotification(EmailStatus.BulkEmailsNotDelivered);
            }

            if (emailViewModel.StatusNotification != null)
                emailViewModel.HasStatusNotification = true;
            return emailViewModel;
        }

        private bool IsResponseOk(Response response)
        {
            if (response.StatusCode.Equals(HttpStatusCode.Accepted)
                   || response.StatusCode.Equals(HttpStatusCode.OK))
            {
                return true;
            }
            return false;
        }

        private NotificationViewModel GetStatusNotification(EmailStatus status, string who = "")
        {
            string title = string.Empty;
            string msg = string.Empty;
            switch (status)
            {
                case EmailStatus.EmailDelivered:
                    title = "Email Sent";
                    msg = $"Your email to {who} has been delivered.";
                    return new NotificationViewModel
                    {
                        Title = title,
                        Message = msg,
                        NotifyType = NotificationType.Success
                    };
                case EmailStatus.EmailNotDelivered:
                    title = "Email Not Sent";
                    msg = $"Your email to {who} was not delivered.";
                    return new NotificationViewModel
                    {
                        Title = title,
                        Message = msg,
                        NotifyType = NotificationType.Failure
                    };
                case EmailStatus.BulkEmailsDelivered:
                    title = "Bulk Emails Sent";
                    msg = $"Your bulk emails were delivered.";
                    return new NotificationViewModel
                    {
                        Title = title,
                        Message = msg,
                        NotifyType = NotificationType.Success
                    };
                case EmailStatus.BulkEmailsNotDelivered:
                    title = "Bulk Emails Not Sent";
                    msg = $"Your bulk emails were not delivered.";
                    return new NotificationViewModel
                    {
                        Title = title,
                        Message = msg,
                        NotifyType = NotificationType.Failure
                    };
            }
            return null;
        }

        private List<EmailContact> GetBulkEmailContacts(EmailViewModel emailViewModel)
        {
            var toAddresses = new List<EmailContact>();
            foreach (var member in emailViewModel.Members)
            {
                var ccgMember = unitOfWork.MemberRepository.FindMemberById(member.MemberId);
                toAddresses.Add(new EmailContact
                {
                    MemberId = member.MemberId,
                    ToEmailAddress = ccgMember.EmailAddress,
                    ReceiverName = ccgMember.FirstName + " " + ccgMember.LastName,
                    DateSent = emailViewModel.EmailContact.DateSent,
                    Subject = emailViewModel.EmailContact.Subject,
                    PlainTextBody = emailViewModel.EmailContact.PlainTextBody
                });
            }
            return toAddresses;
        }
    }

    enum EmailStatus
    {
        EmailDelivered,
        EmailNotDelivered,
        BulkEmailsDelivered,
        BulkEmailsNotDelivered
    }
}