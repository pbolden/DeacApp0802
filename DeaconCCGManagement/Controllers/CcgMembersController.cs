using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security.AntiXss;
using AutoMapper;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Infrastructure;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using PagedList;
using DeaconCCGManagement.PushNotifications;
using DeaconCCGManagement.SmsService;
using System.Configuration;
using System.Threading.Tasks;
using Elmah;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class CcgMembersController : ControllerBase
    {
        private readonly CcgMembersService _service;
        private readonly MemberPhotoService _photoService;
        private readonly PhoneCallService _phoneCallService;
        private readonly CCGService _ccgService;

        public CcgMembersController()
        {
            _service = new CcgMembersService(base.unitOfWork);
            _photoService = new MemberPhotoService(base.unitOfWork);
            _phoneCallService = new PhoneCallService(base.unitOfWork);
            _ccgService = new CCGService(base.unitOfWork);
        }

        // GET: CcgMembers
        public ActionResult Index(int? ccgId, int? page = 1, int? itemsPerPage = 10,
            bool selectAll = false, bool listAll = false, bool getAll = false,
            string query = null, bool allAccess = false,
            int? memberFilter = (int)MemberFilter.None)
        {

            IList<MultiSelectList> items = new List<MultiSelectList>();

            var filter = memberFilter != null ? (MemberFilter)memberFilter : MemberFilter.None;

            #region Set query string params to pass back to view
            // Pass params to view for features such as pagination
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.Page = page;
            ViewBag.CCGId = ccgId;
            ViewBag.GetAll = getAll;
            ViewBag.AllAccess = allAccess;
            ViewBag.IsSelectAll = selectAll;
            ViewBag.MemberFilter = filter;
            ViewBag.Query = query;
            #endregion

            if (ccgId != null)
            {
                var ccg = unitOfWork.CCGRepository.Find(g => g.Id == ccgId);
                ViewBag.CCGName = _ccgService.SetCCGViewName(ccg);
            }

            ActionResult actionResult;
            var members = _service.GetMembersList(ccgId, getAll, query, allAccess, User.Identity.Name, out actionResult).ToList();

            // User may not get authorized and redirected
            if (actionResult != null) return actionResult;

            // Filter members
            _service.FilterMembers(ref members, filter, unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name));

            // Map to view model and return view.
            var memberList = Mapper.Map<IList<ListMembersViewModel>>(members);

            // Validate phone and cell numbers; assign empty string if invalid
            _service.ValidatePhoneNumbers(memberList);

            // Set img src and member-has-photo flag
            _service.SetImgSrcAndHasPhotoFlag(memberList);

            // Set the IsMemberDeacon flag for each member. If user is not member's
            // deacon, also sets the href links for the phone numbers.
            _service.SetIsMemberDeaconFlag(memberList, unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name));

            // List all records in the view on one page
            if (listAll) itemsPerPage = memberList.Count();

            // Select all if 'select all' is selected.
            if (selectAll) _service.SelectAll(memberList);

            // Get CCGs and users for filter menu
            var ccgs = unitOfWork.CCGRepository.FindAll().ToList();

            // Set CCG name with deacon name(s). eg., CCG10 ===> CCG10_Bolden
            _ccgService.SetCCGViewName(ccgs);

            // Add 'All CCGs' option to menu
            ccgs.Insert(0, new CCG { Id = -1, CCGName = "All CCGs" });
            ViewBag.CCGs = new SelectList(ccgs, "Id", "CcgName");


            // Convert view model list to PagedList for pagination.
            return View(memberList.ToPagedList(page ?? 1, itemsPerPage ?? 10));
        }

        // Sets ViewBag properties and returns partial for modal dialog.
        public ActionResult DialogData(int id, string fName, string lName, string number)
        {

            //fName = AntiXssEncoder.HtmlEncode(fName, false);

            var data = new TextOrCallModalDialog
            {
                Id = id,
                FirstName = fName,
                LastName = lName,
                Number = number,
            };

            return PartialView("_ModalDialogContent", data);
        }

        // GET: CcgMembers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ccgMember = unitOfWork.MemberRepository.FindMemberById(id);
            if (ccgMember == null)
            {
                return HttpNotFound();
            }

            var memberDetailsVM = Mapper.Map<DetailsMemberViewModel>(ccgMember);

            _service.SetImgSrcAndHasPhotoFlag(memberDetailsVM);

            var ccg = unitOfWork.CCGRepository.FindById(memberDetailsVM.CCG.Id);

            //concat ccg name with deacon last names
            memberDetailsVM.CCG.CCGName = _ccgService.SetCCGViewName(ccg);


            // If user is not the member's deacon, use href links for phone numbers
            // Users who are not the member's deacon cannot make calls through the app
            memberDetailsVM.IsUserMemberDeacon = AuthHelper.IsMemberDeacon(User.Identity.Name, ccgMember.Id);
            if (!memberDetailsVM.IsUserMemberDeacon)
            {
                memberDetailsVM.HrefPhoneNumberLink
                    = PhoneCallService.GetHrefPhoneNumberLink(ccgMember.PhoneNumber);
                memberDetailsVM.HrefCellNumberLink
                    = PhoneCallService.GetHrefPhoneNumberLink(ccgMember.CellPhoneNumber);
            }
            return View(memberDetailsVM);
        }


        // GET: CcgMembers/Edit/5
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCGMember ccgMember = unitOfWork.MemberRepository.FindMemberById(id);
            if (ccgMember == null)
            {
                return HttpNotFound();
            }

            // Only admins and leadership can edit member data
            if (!AuthHelper.HasAdminAccess(User.Identity.Name))
            {
                return View("EditDenied");
            }
            var ccgs = unitOfWork.CCGRepository.FindAll();
            var ccgsVM = Mapper.Map<IList<CCGViewModel>>(ccgs);
            _ccgService.SetCCGViewName(ccgsVM);
            ViewBag.CCGs = new SelectList(ccgsVM, "Id", "CcgName");

            var ccgMemberVM = Mapper.Map<EditMemberViewModel>(ccgMember);

            return View(ccgMemberVM);
        }

        // POST: CcgMembers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditMemberViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // All member edits should send change request to CR manager

                // Only send change request if admin or member's deacon
                if (AuthHelper.IsMemberDeacon(User.Identity.Name, viewModel.Id) ||
                    AuthHelper.HasAdminAccess(User.Identity.Name))
                {

                    var ccgMemberSelected = unitOfWork.MemberRepository.FindMemberById(viewModel.Id);

                    // concat ccg name with deacon names
                    _ccgService.SetCCGViewName(ccgMemberSelected.CCG);

                    // user logged in
                    var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

                    // map edit view model to change request obj.
                    var changeRequest = Mapper.Map<ChangeRequest>(viewModel);
                    changeRequest.CRDate = DateTime.Now;
                    changeRequest.CcgMemberId = viewModel.CcgMemberId;
                    changeRequest.DeaconId = user.Id;

                    // store change request in database
                    unitOfWork.ChangeRequestRepository.Add(changeRequest);

                    await SendCRManagersMessageAsync(user.FullName);
                    string title = "Change Request Sent";
                    string message = "Your change request has been submitted.";
                    NotifyHelper.SendUserNotification(user.UserName, title, message, type: NotificationType.Success);

                    ViewBag.Message = message;
                }
                else
                {
                    return View("EditDenied");
                }

            }

            ViewBag.CCGs = new SelectList(unitOfWork.CCGRepository.FindAll(), "Id", "CcgName", viewModel.CcgId);

            return Redirect(viewModel.ReturnUrl);
        }


        // GET: CcgMembers/Delete/5
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCGMember ccgMember = unitOfWork.MemberRepository.FindMemberById((int)id);
            if (ccgMember == null)
            {
                return HttpNotFound();
            }

            // Only admins and leadership can delete member
            if (!AuthHelper.HasAdminAccess(User.Identity.Name))
            {
                return View("DeleteDenied");
            }

            return AutoMapView<DeleteMemberViewModel>(ccgMember, View());
        }

        // POST: CcgMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult DeleteConfirmed(DeleteMemberViewModel viewModel)
        {
            // Only allow deleting if the user is admin or leadership
            if (AuthHelper.HasAdminAccess(User.Identity.Name))
            {
                var ccgMember = unitOfWork.MemberRepository.FindById(viewModel.Id);
                unitOfWork.MemberRepository.Delete(ccgMember);
            }

            return Redirect(viewModel.ReturnUrl);
        }


        /// <summary>
        /// Sends change request managers a text message and notification.
        /// </summary>
        /// <param name="crManagers">The change request managers.</param>
        private async Task SendCRManagersMessageAsync(string appUserName)
        {
            string title = "New Change Request";
            string message = $"Deacon {appUserName} has submitted a new change request.";
            string url = Url.Action("Index", "ChangeRequests");
            string linkText = "Change Requests";

            // user(s) assigned to handle change requests 
            var crManagers = unitOfWork.AppUserRepository.FindUsers(u => u.ChangeRequestManager).ToList();

            var smsClient = new TwillioWrapperClient();
            smsClient.Init();

            foreach (var cr in crManagers)
            {
                NotifyHelper.SendUserNotification(cr.UserName, title, message, url, linkText, type: NotificationType.Info);

                var smsMessage = new SmsMessage
                {
                    Message = message,
                    ToNumber = string.IsNullOrEmpty(cr.CellNumber) ? cr.PhoneNumber : cr.CellNumber
                };

                try
                {
                    // Send SMS message
                    await smsClient.SendSmsAsync(smsMessage);
                }
                catch (Exception ex)
                {
                    // log caught exception with Elmah
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }

        }
    }
}
