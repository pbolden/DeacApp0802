using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class PhoneCallController : ControllerBase
    {
        private PhoneCallService _service;

        public PhoneCallController()
        {
            _service = new PhoneCallService(base.unitOfWork);
        }

        // GET: PhoneCall
        // Phone call action method
        public ActionResult CallMember(int? id, string number)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var member = unitOfWork.MemberRepository.FindMemberById(id);
            string memberFullName = $"{member.FirstName} {member.LastName}";

            var phoneCallContact = new PhoneCallContact
            {
                MemberId = (int)id,
                MemberFullName = memberFullName,
                CallDateTime = DateTime.Now,
                PhoneNumber = number,
                HrefPhoneNumberLink = PhoneCallService.GetHrefPhoneNumberLink(number)
            };

            var contactTypes = unitOfWork.ContactTypeRepository.FindAll(ct => ct.Name.Contains("Call"));
            phoneCallContact.ContactTypes = new SelectList(contactTypes, "Id", "Name");

            return View(phoneCallContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CallMember(PhoneCallContact phoneCallContact)
        {
            _service.StorePhoneContact(phoneCallContact, User.Identity.Name);
            ViewBag.ContactSaved = true;

            var contactTypes = unitOfWork.ContactTypeRepository.FindAll(ct => ct.Name.Contains("Call"));
            phoneCallContact.ContactTypes = new SelectList(contactTypes, "Id", "Name");

            return View(phoneCallContact);
        }
      
    }
}