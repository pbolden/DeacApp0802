using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;

namespace DeaconCCGManagement.Services
{
    public class PhoneCallService
    {
        private UnitOfWork db;

        public PhoneCallService(UnitOfWork uow)
        {
            this.db = uow;
        }
      
        /// <summary>
        /// Creates the 'href' link for the phone number
        /// eg., href="tel:+15551234567"
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetHrefPhoneNumberLink(string number)
        {
            if (number == null) return null;

            var sb = new StringBuilder();
            sb.Append("tel:+1");
            foreach (var n in number.Where(char.IsDigit))
            {
                sb.Append(n);
            }

            return sb.ToString();
        }

        public void StorePhoneContact(PhoneCallContact phoneCallContact, string userEmail)
        {
            const string subjectTxt = "Phone call";
            var member = db.MemberRepository.FindMemberById(phoneCallContact.MemberId);

            // To assign user id to contact record property
            var user = db.AppUserRepository.FindUserByEmail(userEmail);

            var contact = new ContactRecord
            {
                CCGMember = member,
                Subject = subjectTxt,
                Comments = phoneCallContact.Comments,
                ContactDate = phoneCallContact.CallDateTime,
                Timestamp = DateTime.Now,
                Duration = phoneCallContact.CallDuration,
                PassAlong = phoneCallContact.PassAlong,
                ContactTypeId = phoneCallContact.ContactTypeId,
                AppUserId = user.Id

            };

            db.ContactRecordRepository.Add(contact);
        }
    }
}