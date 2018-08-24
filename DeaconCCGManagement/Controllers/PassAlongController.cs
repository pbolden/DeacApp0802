using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Infrastructure;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using PagedList;
using DeaconCCGManagement.Helpers;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class PassAlongController : ControllerBase
    {
        private ContactRecordsService _service;

        public PassAlongController()
        {
            _service = new ContactRecordsService(unitOfWork);
        }

        // GET: Pass Along Contacts
        public ActionResult Index()
        {

            //
            // Not used for now. We may delete this.
            //

            return View();
        }

        public ActionResult PassAlongContacts(int? page = 1, int? itemsPerPage = 10,
                   bool listAll = false, bool archive = false,
                   int? sortOption = (int)ContactsSort.DateDescending, string query = null)
        {
            var contactsSort = sortOption == null ? ContactsSort.DateDescending : (ContactsSort)sortOption;

            #region Set params to pass to view
            var passAlongRecordsVM = new PassAlongContactRecordViewModel
            {
                Params = new ActionMethodParams
                {
                    ItemsPerPage = itemsPerPage,
                    Page = page,
                    Archive = archive,
                    ContactsSort = contactsSort,
                    Query = query,
                    ListAll = listAll
                }
            };
            #endregion

            // Get user object
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            var contactRecords = unitOfWork.PassAlongContactRepository
               .FindContactRecords(archive);

            // determine if user is leadership, pastor or admin
            AppUserRole[] roles = new AppUserRole[] { AppUserRole.Admin,
                                                      AppUserRole.DeaconLeadership,
                                                      AppUserRole.Pastor };
            // get all if user is admin, leadership, or pastor
            bool getAll = AuthHelper.IsInRole(User.Identity.Name, roles);

            // if not leadership remove all not in ccg
            if (!getAll)
            {
                contactRecords = contactRecords
                    .Where(c => c.CCGMember.CcgId == user.CcgId);
            }

                // Query contact records
                contactRecords = _service.SearchContactRecords(query, contactRecords).ToList();

                // Sort contact records
                contactRecords = _service.SortContactRecords(contactsSort, contactRecords).ToList();               

                // Map contact records data model to view model
                var contactRecordsVM = Mapper.Map<IList<ContactRecordViewModel>>(contactRecords);

                itemsPerPage = listAll ? contactRecordsVM.Count : itemsPerPage;

                passAlongRecordsVM.ContactRecords = contactRecordsVM.ToPagedList(page ?? 1, itemsPerPage ?? 10);
                _service.TruncateTextAndKeepPrivate(passAlongRecordsVM.ContactRecords, user.Id);

                int notFollowedUpCount = contactRecords.Count(record
                    => string.IsNullOrEmpty(record.PassAlongFollowUpComments));

                ViewBag.NotFollowedUpCount = notFollowedUpCount;
                return View(passAlongRecordsVM);
        }

        public ActionResult ArchiveOrRestoreContact(int id, bool archive = false)
        {
            var record = unitOfWork.PassAlongContactRepository
                .Find(c => c.Id == id);
            record.Archive = archive;
            unitOfWork.PassAlongContactRepository.Update(record);

            //
            //TODO pass model state
            //
            return RedirectToAction("PassAlongContacts");

            //return View("PassAlongContacts");
        }

        // Get
        public ActionResult FollowUp(int id)
        {
            var contactRecord = unitOfWork.ContactRecordRepository.FindById(id);

            // Get user object
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            var viewModel = Mapper.Map<PassAlongFollowUpViewModel>(contactRecord);

            // Keeps the comments private if marked as such
            _service.KeepCommentsPrivate(viewModel, user.Id);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FollowUp(PassAlongFollowUpViewModel viewModel)
        {
            // Get contact record
            var contactRecord = unitOfWork.ContactRecordRepository.FindById(viewModel.Id);

            // update follow comments property
            contactRecord.PassAlongFollowUpComments = viewModel.PassAlongFollowUpComments;

            // update database
            unitOfWork.ContactRecordRepository.Update(contactRecord);
            
            return Redirect(viewModel.ReturnUrl);
        }

        public ActionResult Delete(int? id, bool isFromArchive = false)
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

            ViewBag.IsArchive = isFromArchive;

            return AutoMapView<DeletePassAlongContactViewModel>(contactRecord, View());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DeletePassAlongContactViewModel viewModel, bool isFromArchive = false)
        {
            var record = unitOfWork.PassAlongContactRepository
                .Find(c => c.Id == viewModel.Id);
            if (record != null)
            {
                unitOfWork.PassAlongContactRepository.Delete(record);
            }

            return Redirect(viewModel.ReturnUrl);
        }
    }
}