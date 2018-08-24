using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using System.Web.Routing;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class BulkContactController : ControllerBase
    {
        private readonly BulkContactService _service;
        private readonly CcgMembersService _membersService;

        public BulkContactController()
        {
            _service = new BulkContactService(base.unitOfWork);
            _membersService = new CcgMembersService(base.unitOfWork);
        }

        // GET: BulkContact
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactSelected(int? ccgId, bool getAll, string query, 
            bool allAccess, bool selectAll = false, params int?[] memberIds)
        {

            IEnumerable<CCGMember> members = new List<CCGMember>();
            string userEmail = User.Identity.Name;

            #region Get selected members
            if (selectAll)
            {
                ActionResult actionResult;
                members = _membersService.GetMembersList(ccgId,
                    getAll, query, allAccess, userEmail, out actionResult);

                // User may not get authorized and redirected
                if (actionResult != null) return actionResult;
            }
            else // multi-selected members but not all
            {
                // Get members from data store where id's match those in view model.
                if (memberIds != null && memberIds.Length != 0)
                {
                    members = _membersService.GetSelecteMembers(memberIds);
                }
            }
            #endregion

            // Map list of domain models back to list of view models.
            var selectedMembersVM = Mapper.Map<IList<ListMembersViewModel>>(members);

            // For dropdown list
            var contactTypes = unitOfWork.ContactTypeRepository.GetContactTypesSorted();

            // Deafult selected contact type: 'District Newsletter'
            var defaultSelected = contactTypes
                .SingleOrDefault(ct => ct.Name.Equals("District Newsletter", StringComparison.CurrentCultureIgnoreCase)
                                 || ct.Name.Contains("Newsletter"));

            var bulkContactMembers = new List<BulkContactMember>();

            _service.AddToBulkContactMembers(selectedMembersVM, bulkContactMembers);

            #region Create view model
            var bulkContactViewModel = new BulkContactViewModel
            {
                Members = bulkContactMembers,
                ContactTypes = new SelectList(contactTypes, "Id", "Name", defaultSelected?.Id),
                CreateContactVM = new CreateContactRecordViewModel { ContactDate = DateTime.Now },
                NoMembersSelected = selectedMembersVM.Count == 0
            };
            #endregion

            return View("ContactSelected", bulkContactViewModel);
        }
      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BulkContactViewModel viewModel)
        {
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
            if (viewModel.ContactTypeId == null)
            {
                // temporary; use district newsletter as default
                viewModel.ContactTypeId = 4;
            }
            
            foreach (var member in viewModel.Members)
            {
                //var member = db.MemberRepository.FindMemberById()
                var contact = new ContactRecord
                {
                    AppUserId = user.Id,
                    Archive = false,
                    CCGMemberId = member.MemberId,
                    Comments = viewModel.CreateContactVM.Comments,
                    ContactDate = viewModel.CreateContactVM.ContactDate,
                    ContactTypeId = (int) viewModel.ContactTypeId,
                    Subject = viewModel.CreateContactVM.Subject,
                    Timestamp = DateTime.Now
                };

                StoreContact(contact);
            }
            ViewBag.Complete = true;
            var contactTypes = unitOfWork.ContactTypeRepository.GetContactTypesSorted();
            var defaultSelected = contactTypes
                .SingleOrDefault(ct => ct.Name.Equals("District Newsletter", StringComparison.CurrentCultureIgnoreCase)
                                 || ct.Name.Contains("Newsletter"));

            viewModel.ContactTypes = new SelectList(contactTypes, "Id", "Name", defaultSelected?.Id);

            return View("ContactSelected", viewModel);
        }

        private void StoreContact(ContactRecord contactRecord)
        {
            unitOfWork.ContactRecordRepository.Add(contactRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkEmail(BulkContactViewModel viewModel)
        {
            if (viewModel.Members.Count == 0)
                return RedirectToAction("ContactSelected");

            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            var emailContact = new EmailContact
            {
                FromEmailAddress = user.Email,
                ToEmailAddress = "temp@ccg.com",
                SenderName = user.FullName,
                DateSent = DateTime.Now,
            };

            var emailViewModel = new EmailViewModel
            {
                EmailContact = emailContact,
                Bulk = true,
                Members = viewModel.Members
            };

            TempData["EmailViewModel"] = emailViewModel;
            return RedirectToAction("SendBulkEmail", "Email");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BulkTextMessage(BulkContactViewModel viewModel)
        {
            if (viewModel.Members.Count == 0)            
                return RedirectToAction("ContactSelected");            

            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            var textMessageContact = new TextMessageContact
            {
                ToPhoneNumber = string.Empty,
                FromPhoneNumber = user.PhoneNumber,
                DateSent = DateTime.Now
            };

            var textMessageViewModel = new TextMessageViewModel
            {
                TextMessageContact = textMessageContact,
                Bulk = true,
                Members = viewModel.Members
            };

            TempData["TextMessageViewModel"] = textMessageViewModel;

            RouteValueDictionary routeDictionary = new RouteValueDictionary();
            routeDictionary.Add("TextMessageViewModel", textMessageViewModel);

            return RedirectToAction("SendBulkTextMessage", "Sms", routeDictionary);
        }
    }
}