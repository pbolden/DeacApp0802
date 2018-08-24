using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using PagedList;
using DeaconCCGManagement.PushNotifications;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class ContactRecordsController : ControllerBase
    {
        private readonly ContactRecordsService _service;
        private readonly CCGService _ccgService;

        public ContactRecordsController()
        {
            _service = new ContactRecordsService(unitOfWork);
            _ccgService = new CCGService(unitOfWork);
        }

        // GET: ContactRecords
        public ActionResult Index(int? memberId, int? ccgId,
            int? page = 1, int? itemsPerPage = 10, bool getAll = false, bool archive = false,
            int? sortOption = (int)ContactsSort.DateDescending, string query = null,
            int? dateRangeFilter = (int)DateRangeFilter.ThreeMonths)
        {
            var dateFilter = dateRangeFilter == null ? DateRangeFilter.ThreeMonths : (DateRangeFilter)dateRangeFilter;
            var contactsSort = sortOption == null ? ContactsSort.DateDescending : (ContactsSort)sortOption;

            #region Set params to pass to view
            var listViewModel = new ListContactRecordViewModel
            {
                Params = new ActionMethodParams
                {
                    MemberId = memberId,
                    CCGId = ccgId,
                    ItemsPerPage = itemsPerPage,
                    Page = page,
                    GetAll = getAll,
                    Archive = archive,
                    ContactsSort = contactsSort,
                    DateRangeFilter = dateFilter,
                    Query = query,

                }
            };
            #endregion

            //var contactRecords = new List<ContactRecord>();

            // Get user object
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            // Used in the view to check permission to edit 
            listViewModel.Params.UserId = user.Id;

            // Get contact records
            ActionResult actionResult;
          
            var contactRecords = _service.GetContactRecords(memberId, ccgId, getAll, archive, 
                user, dateFilter, query, out actionResult);

            if (actionResult != null) return actionResult;

            #region sort, truncate, keep private, and map models

            contactRecords = _service.SortContactRecords(contactsSort, contactRecords);

            var contactRecordsVM = Mapper.Map<IList<ContactRecordViewModel>>(contactRecords);
            
            listViewModel.ContactRecords = contactRecordsVM.ToPagedList(page ?? 1, itemsPerPage ?? 10);


            // Truncates the subject and comments
            // Keeps comments private if marked as such
            _service.TruncateTextAndKeepPrivate(listViewModel.ContactRecords, user.Id);

            #endregion

            // Get CCGs and users for filter menu
            var ccgs = unitOfWork.CCGRepository.FindAll().ToList();

            // Set CCG name with deacon name(s). eg., CCG10 ===> CCG10_Bolden
            _ccgService.SetCCGViewName(ccgs);

            // Add 'All CCGs' option to menu
            ccgs.Insert(0, new CCG { Id = -1, CCGName = "All CCGs" });
            listViewModel.CCGs = new SelectList(ccgs, "Id", "CcgName");

            if (ccgId != null && ccgId != -1)
            {
                var ccg = unitOfWork.CCGRepository.Find(g => g.Id == ccgId);
                listViewModel.CcgName = ccg.CCGName;
            }

            return View(listViewModel);
        }

        public ActionResult ArchiveOrRestoreContact(int id, bool archive = false)
        {
            var record = unitOfWork.ContactRecordRepository.Find(c => c.Id == id);
            record.Archive = archive;
            unitOfWork.ContactRecordRepository.Update(record);


            //
            //TODO pass model state
            //
            return RedirectToAction("Index");

            //return View("Index");
        }

        // GET: ContactRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ContactRecord contactRecord = unitOfWork.ContactRecordRepository.FindById(id);
            if (contactRecord == null)
            {
                return HttpNotFound();
            }

            var contactRecordVM = Mapper.Map<DetailsContactRecordViewModel>(contactRecord);

            if (contactRecordVM.Private)
            {
                // Get user object
                var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
                _service.KeepCommentsPrivate(contactRecordVM, user.Id);
            }

            return View(contactRecordVM);
        }

        // GET: ContactRecords/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contactTypes = unitOfWork.ContactTypeRepository.GetContactTypesSorted();
            var member = unitOfWork.MemberRepository.FindMemberById(id);

            // Default value for drop down list
            var contactType = unitOfWork.ContactTypeRepository.Find(ct => ct.Name.Equals("Church"));
            var viewModel = new CreateContactRecordViewModel
            {
                CCGMemberId = (int)id,
                ContactDate = DateTime.Now,
                MemberFullName = $"{member.FirstName} {member.LastName}",
                ContactTypeId = contactType.Id,
                ContactTypes = new SelectList(contactTypes, "Id", "Name")
            };

            return View(viewModel);
        }

        // POST: ContactRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateContactRecordViewModel viewModel)
        {
            // Ensures records displayed even if subject/comments are empty.
            // The search query searches for "" if none is given by user.
            if (viewModel.Subject == null) viewModel.Subject = "";
            if (viewModel.Comments == null) viewModel.Comments = "";

            // Sanitize data input with AntiXssEncoder
            // eg.: example: " ==> &quot;
            viewModel = (CreateContactRecordViewModel)_service.SanitizeContactRecordViewModel(viewModel);

            var contactRecord = Mapper.Map<ContactRecord>(viewModel);           

            if (ModelState.IsValid)
            {
                // Set timestamp
                contactRecord.Timestamp = DateTime.Now;

                // Assign user id to contact record property
                var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
                contactRecord.AppUserId = user.Id;

                unitOfWork.ContactRecordRepository.Add(contactRecord);

                if (contactRecord.PassAlong)
                {
                    // TODO should this go to leadership and pastor?
                    // Send pastor a notification
                    var pastors = unitOfWork.AppUserRepository.FindUsersByRole(AppUserRole.Pastor);
                    foreach (var pastor in pastors)                    
                        NotifyOfPassAlongContact(pastor.UserName, contactRecord.DeaconFullName);                   

                }

                return Redirect(viewModel.ReturnUrl);
            }

            var contactTypes = unitOfWork.ContactTypeRepository.GetContactTypesSorted();
            viewModel.ContactTypes = new SelectList(contactTypes, "Id", "Name");

            return View(viewModel);
        }

        private void NotifyOfPassAlongContact(string username, string deaconName)
        {
            string title = "Review Contact Record";
            string message = $"Please review this contact record from {deaconName}.";
            string url = Url.Action("PassAlongContacts", "PassAlong");
            string linkText = "Review Contacts";

            NotifyHelper.SendUserNotification(username, title, 
                message, url, linkText, PushNotifications.NotificationType.Info);

        }

        // GET: ContactRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contactRecord = unitOfWork.ContactRecordRepository.FindById(id);
            if (contactRecord == null)
            {
                return HttpNotFound();
            }

            // Only the user who made the contact can edit it
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
            if (user.Id != contactRecord.AppUserId)
            {
                return View("EditDenied");
            }

            var viewModel = Mapper.Map<EditContactRecordViewModel>(contactRecord);
            var contactTypes = unitOfWork.ContactTypeRepository.GetContactTypesSorted();
            viewModel.ContactTypes = new SelectList(contactTypes, "Id", "Name");

            return View(viewModel);
        }

        // POST: ContactRecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditContactRecordViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            // Sanitize data input with AntiXssEncoder
            viewModel = (EditContactRecordViewModel)_service.SanitizeContactRecordViewModel(viewModel);

            var editedContactRecord = Mapper.Map<ContactRecord>(viewModel);

            var contactRecord = unitOfWork.ContactRecordRepository.FindById(editedContactRecord.Id);
            editedContactRecord.ContactType = unitOfWork.ContactTypeRepository.FindById(editedContactRecord.ContactTypeId);

            // Update contact record
            _service.UpdateContactRecord(contactRecord, editedContactRecord);

            // If 'PassAlong' is false go to Index
            if (!contactRecord.PassAlong) return Redirect(viewModel.ReturnUrl);

            // If 'PassAlong' is true, get 'pass along' object
            var passAlongContact = unitOfWork.PassAlongContactRepository.FindById(contactRecord.Id);

            // Ensures email doesn't get sent to the pastor/leadership more than once
            // User can check/uncheck 'Pass Along' in edit view
            if (passAlongContact.PassAlongEmailSent) return Redirect(viewModel.ReturnUrl);

            // Send email to pastor/leadership if haven't already
            // This may occur if deacon doesn't check 'PassAlong'
            // initially but does so in edit
            //var sendGridService = new SendGridEmailService();
            //sendGridService.SendPassAlongContact(contactRecord, db);

            return Redirect(viewModel.ReturnUrl);
        }

        // GET: ContactRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactRecord contactRecord = unitOfWork.ContactRecordRepository.FindById(id);
            if (contactRecord == null)
            {
                return HttpNotFound();
            }

            // Only the user who made the contact or and admin can delete it
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
            if (user.Id != contactRecord.AppUserId && !AuthHelper.IsInRole(user.Email, AppUserRole.Admin) && !AuthHelper.IsInRole(user.Email, AppUserRole.DeaconLeadership))
            {
                return View("DeleteDenied");
            }

            return AutoMapView<DeleteContactRecordViewModel>(contactRecord, View());
        }

        // POST: ContactRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, DeleteContactRecordViewModel viewModel)
        {
            ContactRecord contactRecord = unitOfWork.ContactRecordRepository.FindById(id);
            unitOfWork.ContactRecordRepository.Delete(contactRecord);

            return Redirect(viewModel.ReturnUrl);
        }
    }
}
