using System;
using System.Web.Mvc;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Net;
using DeaconCCGManagement.SendEmailService;
using System.Threading.Tasks;
using System.Collections.Generic;
using SendGrid;
using DeaconCCGManagement.Utilities;
using DeaconCCGManagement.PushNotifications;
using DeaconCCGManagement.Helpers;

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

            return View(emailViewModel);
        }

        public ActionResult SendBulkEmail()
        {
            var emailViewModel = TempData["EmailViewModel"];
            return View("SendEmail", emailViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendEmail(EmailViewModel emailViewModel)
        {
            if (!ModelState.IsValid)
                return View(emailViewModel);

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
                await SendBulkEmailsAsync(emailViewModel, client);

            else // send email to one member 
                await SendSingleEmailAsync(emailViewModel, client);

            return View("EmailComplete");
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

        private async Task SendSingleEmailAsync(EmailViewModel emailViewModel, EmailWrapperClient client)
        {

            //
            //TODO DEBUG
            //
#if DEBUG
            //StoreEmailContact(emailViewModel.EmailContact);
#endif


            Response response;
            try
            {
                response = await client.SendSingleEmailAsync(emailViewModel.EmailContact);

                if (IsResponseOk(response))
                {
                    NotifyUserOfStatus(EmailStatus.EmailDelivered, emailViewModel.EmailContact.ReceiverName);
                    emailViewModel.EmailContact.DateSent = DateTime.Now;
                    StoreEmailContact(emailViewModel.EmailContact);
                }
                else                
                    NotifyUserOfStatus(EmailStatus.EmailNotDelivered, emailViewModel.EmailContact.ReceiverName);
                
            }
            catch (Exception ex)
            {
                NotifyUserOfStatus(EmailStatus.EmailNotDelivered, emailViewModel.EmailContact.ReceiverName);
            }


        }

        private async Task SendBulkEmailsAsync(EmailViewModel emailViewModel, EmailWrapperClient client)
        {
            Response response;
            var bulkContacts = GetBulkEmailContacts(emailViewModel);

            //
            //TODO DEBUG
            //
#if DEBUG
            //StoreBulkEmailContacts(bulkContacts);
#endif



            try
            {
                response = await client.SendMultipleEmailsAsync(bulkContacts, emailViewModel.EmailContact);
                if (IsResponseOk(response))
                {
                    NotifyUserOfStatus(EmailStatus.BulkEmailsDelivered);
                    emailViewModel.EmailContact.DateSent = DateTime.Now;
                    StoreBulkEmailContacts(bulkContacts);
                }
                else
                {
                    NotifyUserOfStatus(EmailStatus.BulkEmailsNotDelivered);
                }

            }
            catch (Exception ex)
            {
                NotifyUserOfStatus(EmailStatus.BulkEmailsNotDelivered);
            }
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

        private void NotifyUserOfStatus(EmailStatus status, string who = "")
        {
            string title = string.Empty;
            string msg = string.Empty;
            switch (status)
            {
                case EmailStatus.EmailDelivered:
                    title = "Email Sent";
                    msg = $"Your email to {who} has been delivered.";
                    NotifyHelper.SendUserNotification(User.Identity.Name,
                        title, msg, type: NotificationType.Success);
                    break;
                case EmailStatus.EmailNotDelivered:
                    title = "Email Not Sent";
                    msg = $"Your email to {who} was not delivered.";
                    NotifyHelper.SendUserNotification(User.Identity.Name,
                        title, msg, type: NotificationType.Failure);
                    break;
                case EmailStatus.BulkEmailsDelivered:
                    title = "Bulk Emails Sent";
                    msg = $"Your bulk emails were delivered.";
                    NotifyHelper.SendUserNotification(User.Identity.Name,
                        title, msg, type: NotificationType.Success);
                    break;
                case EmailStatus.BulkEmailsNotDelivered:
                    title = "Bulk Emails Not Sent";
                    msg = $"Your bulk emails were not delivered.";
                    NotifyHelper.SendUserNotification(User.Identity.Name,
                        title, msg, type: NotificationType.Failure);
                    break;
            }
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