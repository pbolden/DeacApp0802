using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using PagedList;
using DeaconCCGManagement.PushNotifications;

namespace DeaconCCGManagement.Controllers
{
    public class PrayerRequestController : ControllerBase
    {
        private readonly PrayerRequestService _service;
        private readonly CCGService _ccgService;

        public PrayerRequestController()
        {
            _service = new PrayerRequestService(base.unitOfWork);
            _ccgService = new CCGService(base.unitOfWork);
        }

        // GET: 
        public ActionResult Index(int? memberId, int? ccgId,
            int? dateRangeFilter = (int)DateRangeFilter.TwoWeeks,
            int? sortOption = (int)ContactsSort.DateDescending,
            int? page = 1, int? itemsPerPage=10, bool getAll=false, string query=null)
        {
            // Cast int passed by route to an enum
            var dateFilter = dateRangeFilter != null ? (DateRangeFilter)dateRangeFilter : DateRangeFilter.TwoWeeks;
            var contactsSort = sortOption == null ? ContactsSort.DateDescending : (ContactsSort)sortOption;

            #region Set params to pass to view
            var prayerRequestsList = new PrayerRequestListViewModel
            {
                Params = new ActionMethodParams
                {
                    MemberId = memberId,
                    CCGId = ccgId,
                    ItemsPerPage = itemsPerPage,
                    Page = page,
                    GetAll = getAll,
                    DateRangeFilter = dateFilter,
                    Query = query,
                    ContactsSort = contactsSort
                }
            };
            #endregion

            // Get principal user obj
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            if (memberId != null)
            {
                var member = unitOfWork.MemberRepository.FindMemberById(memberId);
                ViewBag.MemberName = $"{member.FirstName} {member.LastName}"; 
            }

            DateTime dateTimeOffset;
            _service.GetOffsetDate(dateFilter, out dateTimeOffset);

            List<ContactRecord> prayerRequests;


            // determine if user is leadership, pastor or admin
            AppUserRole[] roles = new AppUserRole[] { AppUserRole.Admin,
                                                      AppUserRole.DeaconLeadership,
                                                      AppUserRole.Pastor };
            // get all if user is admin, leadership, or pastor
            getAll = AuthHelper.IsInRole(User.Identity.Name, roles);

            // not leadership so only get prayer requests within ccg
            if (!getAll && ccgId == null) ccgId = user.CcgId;

            prayerRequests = _service.GetPrayerRequests(memberId, 
                dateTimeOffset, ccgId, getAll, user, query).ToList();

            // Sort prayer requests
            prayerRequests = _service.SortContactRecords(contactsSort, prayerRequests).ToList();

            // Used to check permission to edit in the view
            prayerRequestsList.Params.UserId = user.Id;

            var prayerRequestsVM = Mapper.Map<IList<PrayerRequestViewModel>>(prayerRequests);

            prayerRequestsList.PrayerRequests = prayerRequestsVM.ToPagedList(page ?? 1, itemsPerPage ?? 10);


            // Truncates the subject and comments
            // Keeps comments private if marked as such
            _service.TruncateTextAndKeepPrivate(prayerRequestsList.PrayerRequests, user.Id);

            // Get CCGs for filter menu
            var ccgs = unitOfWork.CCGRepository.FindAll().ToList();

            // Add 'All CCGs' option to menu
            _ccgService.SetCCGViewName(ccgs);
            ccgs.Insert(0, new CCG { Id = -1, CCGName = "All CCGs" });
            prayerRequestsList.CCGs = new SelectList(ccgs, "Id", "CcgName");

            if (ccgId != null && ccgId != -1)
            {
                var ccg = unitOfWork.CCGRepository.Find(g => g.Id == ccgId);
                prayerRequestsList.CcgName = ccg.CCGName;
            }

            return View(prayerRequestsList);
        }



        // GET: PrayerRequestViewModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var prayerRequest = unitOfWork.ContactRecordRepository.FindById(id);

            if (prayerRequest == null)
            {
                return HttpNotFound();
            }

            // Get principal user
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

            var prayerRequestVM = Mapper.Map<PrayerRequestViewModel>(prayerRequest);

            // Keep comments private for other users if marked as such
            _service.KeepCommentsPrivate(prayerRequestVM, user.Id);

            return View(prayerRequestVM);
        }

        // GET: PrayerRequestViewModels/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var member = unitOfWork.MemberRepository.FindMemberById(id);
            var viewModel = new PrayerRequestViewModel
            {
                CCGMemberId = (int)id,
                ContactDate = DateTime.Now,
                MemberFullName = $"{member.FirstName} {member.LastName}"
            };
            return View(viewModel);
        }

        // POST: PrayerRequestViewModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PrayerRequestViewModel prayerRequestViewModel)
        {
            if (!ModelState.IsValid) return View(prayerRequestViewModel);

            // Ensures records displayed even if subject/comments are empty.
            // The search query searches for "" if none is given by user.
            if (prayerRequestViewModel.Subject == null) prayerRequestViewModel.Subject = "";
            if (prayerRequestViewModel.Comments == null) prayerRequestViewModel.Comments = "";

            // Sanitize data input with AntiXssEncoder
            prayerRequestViewModel = _service.SanitizePrayerRequestViewModel(prayerRequestViewModel);

            var contactRecord = Mapper.Map<ContactRecord>(prayerRequestViewModel);

            // Used to find contact type object
            string contacTypePR = "Prayer Request";

            // Get contact type object that matches 'Prayer Request'
            var contactType = unitOfWork.ContactTypeRepository
                .Find(t => t.Name.Equals(contacTypePR, StringComparison.CurrentCultureIgnoreCase));

            contactRecord.ContactTypeId = contactType.Id;
            contactRecord.Timestamp = DateTime.Now;

            // Assign user id to contact record property
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
            contactRecord.AppUserId = user.Id;

            unitOfWork.ContactRecordRepository.Add(contactRecord);

            // Send pastor/leadership a notification about prayer request
            NotifyOfPrayerRequest(contactRecord.DeaconFullName);

            return Redirect(prayerRequestViewModel.ReturnUrl);
        }

        private void NotifyOfPrayerRequest(string deaconName)
        {
            string title = "New Prayer Request";
            string message = $"Deacon {deaconName} has submitted a new prayer request.";
            string url = Url.Action("Index", "PrayerRequest", new { getAll = true });
            string linkText = "Prayer Requests";

            var appUsers = unitOfWork.AppUserRepository.FindAll();

            foreach (var appUser in appUsers)
            {
                if (appUser.UserName != User.Identity.Name)
                {
                    NotifyHelper.SendUserNotification(appUser.UserName, title,
                        message, url, linkText, PushNotifications.NotificationType.Info);
                }
            }

        }

        // GET: PrayerRequestViewModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var prayerRequest = unitOfWork.ContactRecordRepository.FindById(id);
            if (prayerRequest == null)
            {
                return HttpNotFound();
            }

            // Only the user who made the contact can edit it
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
            if (user.Id != prayerRequest.AppUserId)
            {
                return View("EditDenied");
            }

            // Map ContactRecord to PrayerRequestViewModel and return to view
            return AutoMapView<PrayerRequestViewModel>(prayerRequest, View());
        }

        // POST: PrayerRequestViewModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PrayerRequestViewModel viewModel)
        {
            // Sanitize data input with AntiXssEncoder
            viewModel = _service.SanitizePrayerRequestViewModel(viewModel);

            if (ModelState.IsValid)
            {
                var contactRecord = unitOfWork.ContactRecordRepository.FindById(viewModel.Id);

                // Only the user who made the contact can edit it
                var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
                if (user.Id != contactRecord.AppUserId)
                {
                    return View("EditDenied");
                }

                // update properties
                contactRecord.ContactDate = viewModel.ContactDate;
                contactRecord.Private = viewModel.Private;
                contactRecord.Subject = viewModel.Subject;
                contactRecord.Comments = viewModel.Comments;

                unitOfWork.ContactRecordRepository.Update(contactRecord);

                return Redirect(viewModel.ReturnUrl);
            }

            return View(viewModel);
        }

        // GET: PrayerRequestViewModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var prayerRequest = unitOfWork.ContactRecordRepository.FindById(id);
            if (prayerRequest == null)
            {
                return HttpNotFound();
            }

            // Only the user who made the PR or and admin can delete it
            var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);
            if (user.Id != prayerRequest.AppUserId
                && !AuthHelper.HasAdminAccess(user.Email))
            {
                return View("DeleteDenied");
            }

            // Map ContactRecord to PrayerRequestViewModel and return to view
            return AutoMapView<PrayerRequestViewModel>(prayerRequest, View());
        }

        // POST: PrayerRequestViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(PrayerRequestViewModel viewModel)
        {
            var prayerRequest = unitOfWork.ContactRecordRepository.FindById(viewModel.Id);
            unitOfWork.ContactRecordRepository.Delete(prayerRequest);

            return Redirect(viewModel.ReturnUrl);
        }
    }
}
