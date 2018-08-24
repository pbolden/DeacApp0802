using System;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.SmsService;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Collections.Generic;
using DeaconCCGManagement.PushNotifications;
using Elmah;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class SmsController : ControllerBase
    {
        // GET
        public ActionResult SendTextMessage(int? id, string number)
        {
            if (id == null)            
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            

            var member = unitOfWork.MemberRepository.FindMemberById(id);
            string memberFullName = $"{member.FirstName} {member.LastName}";

            var contact = new TextMessageContact
            {
                MemberId = (int)id,
                MemberFullName = memberFullName,
                ToPhoneNumber = number,
            };

            var viewModel = new TextMessageViewModel
            {
                TextMessageContact = contact,
                Bulk = false
            };


            // Check if testing Twillio
            bool testTwillio = bool.Parse(ConfigurationManager.AppSettings["TestTwillio"]);
            if (testTwillio)
            {
                viewModel.IsTest = testTwillio;
                viewModel.TestFromNumber = ConfigurationManager.AppSettings["TwillioFromNumber"];
            }

            viewModel.HasStatusNotification = false;
            viewModel.StatusNotification = new NotificationViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendTextMessage(TextMessageViewModel viewModel)
        {
            if (viewModel == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var smsClient = new TwillioWrapperClient();
            smsClient.Init();           

            // Get 'from number'
            string fromNumber = GetFromNumber(viewModel);     

            if (!viewModel.Bulk)            
                await SendSingleTextMessage(viewModel, smsClient, fromNumber);            
            else            
                await SendBulkTextMessages(viewModel,smsClient, fromNumber);            

            viewModel.TextMessageSent = true;

            return View(viewModel);
        }

        public ActionResult SendBulkTextMessage(RouteValueDictionary routeDictionary=null)
        {
            var viewModel = TempData["TextMessageViewModel"];

            if (viewModel == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("SendTextMessage", viewModel);
        }

        private async Task<ActionResult> SendSingleTextMessage(TextMessageViewModel viewModel, TwillioWrapperClient smsClient, string fromNumber)
        {
            var smsMessage = new SmsMessage
            {
                Message = viewModel.TextMessageContact.Message,
                FromNumber = fromNumber
            };

            // set 'to' number
            if (!string.IsNullOrEmpty(viewModel.TestToNumber))
                smsMessage.ToNumber = viewModel.TestToNumber;
            else if (!string.IsNullOrEmpty(viewModel.TextMessageContact.ToPhoneNumber))
                smsMessage.ToNumber = viewModel.TextMessageContact.ToPhoneNumber;

            try
            {
                // Send SMS message
                IResponse response = await smsClient.SendSmsAsync(smsMessage);
                if (IsResponseOk(response))
                {
                    viewModel.StatusNotification = GetStatusNotification(TextMessageStatus.SingleTextDelivered, viewModel.TextMessageContact.MemberFullName);

                    viewModel.TextMessageContact.DateSent = DateTime.Now;

                    // Store contact record
                    StoreTextMessage(viewModel.TextMessageContact);
                }
                else
                    viewModel.StatusNotification = GetStatusNotification(TextMessageStatus.SingleTextNotDelivered, viewModel.TextMessageContact.MemberFullName);
                

            }
            catch (Exception ex)
            {
                // log caught exception with Elmah
                ErrorSignal.FromCurrentContext().Raise(ex);

                viewModel.StatusNotification = GetStatusNotification(TextMessageStatus.SingleTextNotDelivered, viewModel.TextMessageContact.MemberFullName);
            }

            if (viewModel.StatusNotification != null)
                viewModel.HasStatusNotification = true;

            return View(viewModel);
        }

        private async Task<ActionResult> SendBulkTextMessages(TextMessageViewModel viewModel, TwillioWrapperClient smsClient, string fromNumber)
        {
            bool isGoodResponse = true;
            foreach (var member in viewModel.Members)
            {
                var ccgMember = unitOfWork.MemberRepository.FindMemberById(member.MemberId);

                string toNumber = string.Empty;

                // check if cell number exists first 
                if (!string.IsNullOrEmpty(ccgMember.CellPhoneNumber))
                    toNumber = ccgMember.CellPhoneNumber;
                else
                    toNumber = ccgMember.PhoneNumber;

                // use test number is exists
                if (!string.IsNullOrEmpty(viewModel.TestToNumber))               
                    toNumber = viewModel.TestToNumber;

                var smsMessage = new SmsMessage
                {
                    ToNumber = toNumber,
                    FromNumber = fromNumber,
                    Message = viewModel.TextMessageContact.Message,
                };

                try
                {
                    // Send SMS message
                    IResponse response = await smsClient.SendSmsAsync(smsMessage);
                    if (IsResponseOk(response))
                    {   
                        viewModel.TextMessageContact.DateSent = DateTime.Now;

                        // Store contact record
                        viewModel.TextMessageContact.MemberId = member.MemberId;
                        StoreTextMessage(viewModel.TextMessageContact);
                    }
                    else                    
                        isGoodResponse = false;
                }
                catch (Exception ex)
                {
                    // log caught exception with Elmah
                    ErrorSignal.FromCurrentContext().Raise(ex);

                    isGoodResponse = false;
                }
            }

            // Send notification to user with bulk text message status.
            // If any text fails user gets a not delivered message.           
            if (isGoodResponse)
                viewModel.StatusNotification = GetStatusNotification(TextMessageStatus.BulkTextsDelivered, viewModel.TextMessageContact.MemberFullName);
            else
                viewModel.StatusNotification = GetStatusNotification(TextMessageStatus.BulkTextsNotDelivered, viewModel.TextMessageContact.MemberFullName);

            if (viewModel.StatusNotification != null)
                viewModel.HasStatusNotification = true;

            return View(viewModel);
        }

        private void StoreTextMessage(TextMessageContact textMessageContact)
        {
            const string subjectTxt = "Text message";
            var member = unitOfWork.MemberRepository.FindMemberById(textMessageContact.MemberId);
            var contactType = unitOfWork.ContactTypeRepository.Find(ct => ct.Name.Equals("Text Messaging"));

            // To assign user id to contact record property
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            var contact = new ContactRecord
            {
                CCGMemberId = member.Id,
                Subject = subjectTxt,
                Comments = textMessageContact.Message,
                ContactDate = DateTime.Now,
                Timestamp = DateTime.Now,
                ContactTypeId = contactType.Id,
                AppUserId = user.Id
            };

            unitOfWork.ContactRecordRepository.Add(contact);
        }

        private void StoreBulkTextMessages(List<TextMessageContact> texts)
        {
            foreach (var text in texts)
            {
                StoreTextMessage(text);
            }
        }

        private string GetFromNumber(TextMessageViewModel viewModel)
        {
            bool testTwillio = bool.Parse(ConfigurationManager.AppSettings["TestTwillio"]);
            string fromNumber = ConfigurationManager.AppSettings["TwillioFromNumber"];
            if (testTwillio)
            {   
                // gives user the option to enter and test another from number
                if (!string.IsNullOrEmpty(viewModel.TestFromNumber)
                    && !viewModel.TestFromNumber.Equals(fromNumber))
                    return viewModel.TestFromNumber;
            }

            // production            
            return fromNumber;            
        }

        private bool IsResponseOk(IResponse response)
        {    
            //
            // TODO test this
            //
            if (response.Status.Equals("queued"))
            {
                return true;
            }
            return false;
        }

        private NotificationViewModel GetStatusNotification(TextMessageStatus status, string who = "")
        {
            string title = string.Empty;
            string msg = string.Empty;

            switch (status)
            {
                case TextMessageStatus.SingleTextDelivered:
                    title = "Text Message Sent";
                    msg = $"Your text message to {who} has been delivered.";
                    return new NotificationViewModel
                    {
                        Title = title,
                        Message = msg,
                        NotifyType = NotificationType.Success
                    };
                    //NotifyHelper.SendUserNotification(User.Identity.Name,
                    //    title, msg, type: NotificationType.Success);
                   
                case TextMessageStatus.SingleTextNotDelivered:
                    title = "Text Message Not Sent";
                    msg = $"Your text message to {who} was not delivered.";
                    return new NotificationViewModel
                    {
                        Title = title,
                        Message = msg,
                        NotifyType = NotificationType.Failure
                    };
                    //NotifyHelper.SendUserNotification(User.Identity.Name,
                    //    title, msg, type: NotificationType.Failure);
                    //break;
                case TextMessageStatus.BulkTextsDelivered:
                    title = "Bulk Text Messages Sent";
                    msg = $"Your bulk text messages were delivered.";
                    return new NotificationViewModel
                    {
                        Title = title,
                        Message = msg,
                        NotifyType = NotificationType.Success
                    };
                    //NotifyHelper.SendUserNotification(User.Identity.Name,
                    //    title, msg, type: NotificationType.Success);
                    //break;
                case TextMessageStatus.BulkTextsNotDelivered:
                    title = "Bulk text Messages Not Sent";
                    msg = $"Your bulk text messages were not delivered.";
                    return new NotificationViewModel
                    {
                        Title = title,
                        Message = msg,
                        NotifyType = NotificationType.Failure
                    };
                    //NotifyHelper.SendUserNotification(User.Identity.Name,
                    //    title, msg, type: NotificationType.Failure);
                    //break;
            }

            return null;
        }

    }

    enum TextMessageStatus
    {
        SingleTextDelivered,
        SingleTextNotDelivered,
        BulkTextsDelivered,
        BulkTextsNotDelivered
    }
}