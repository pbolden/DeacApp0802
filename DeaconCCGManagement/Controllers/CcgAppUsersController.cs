using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Infrastructure;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using PagedList;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class CcgAppUsersController : ControllerBase
    {
        private readonly CcgAppUsersService _service;
        private readonly CCGService _ccgService;

        public CcgAppUsersController()
        {
            _service = new CcgAppUsersService(base.unitOfWork);
            _ccgService = new CCGService(base.unitOfWork);
        }

        // GET: CcgAppUsers
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Index(int? ccgId, int? page=1, int? itemsPerPage=10,
               bool listAll=false, string query=null, int? appUserFilter = (int)AppUserFilter.AllAppUsers)
        {
            #region Set params to pass to view
            var actionParams = new ActionMethodParams
            {
                ItemsPerPage = itemsPerPage,
                Page = page,
                Query = query,
            };
            #endregion

            List<CCGAppUser> users;

            // Get users in a CCG or all users
            users = ccgId != null ? unitOfWork.AppUserRepository.FindUsers(u => u.CcgId == ccgId).ToList() 
                : unitOfWork.AppUserRepository.FindUsers().ToList();

            // Filter app users
            var filterOption = appUserFilter != null ?
                (AppUserFilter)appUserFilter : AppUserFilter.AllAppUsers;
            users = _service.FilterAppUsers(users, filterOption);

            if (query != null)
                users = _service.SearchUsers(query);

            // Map list of users to list of view models
            var usersList = Mapper.Map<IList<CcgAppUserViewModel>>(users);

            // Set user has-photo flag and img src
            _service.SetImgSrcAndHasPhotoFlag(usersList);

            itemsPerPage = listAll ? usersList.Count : itemsPerPage;
            var viewModel = new ListCcgAppUsersViewModel
            {
                AppUsers = usersList.ToPagedList(page ?? 1, itemsPerPage ?? 10),
                Params = actionParams,
                AppUserFilter = appUserFilter != null ? (AppUserFilter)appUserFilter : AppUserFilter.AllAppUsers
            };

            _service.SetHrefLinkForPhoneNumbers(viewModel.AppUsers);

            return View(viewModel);
        }

        // Allow pastor and deacons to view deacon's contact info
        [Authorize]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CCGAppUser ccgAppUser = unitOfWork.AppUserRepository.FindUserById(id);

            if (ccgAppUser == null)
            {
                return HttpNotFound();
            }

            ViewBag.Roles = unitOfWork.AppUserRepository.GetUserRoles(ccgAppUser.Id);

            var viewModel = Mapper.Map<DetailsCcgAppUserViewModel>(ccgAppUser);

            _service.SetHrefLinkForPhoneNumbers(viewModel);
            _service.SetImgSrcAndHasPhotoFlag(viewModel);

            return View(viewModel);
        }

        // GET: CcgAppUsers/Create
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Create()
        {
            var ccgs = unitOfWork.CCGRepository.FindAll();
            var ccgsVM = Mapper.Map<IList<CCGViewModel>>(ccgs);
            _ccgService.SetCCGViewName(ccgsVM);
            ViewBag.CCGs = new SelectList(ccgsVM, "Id", "CcgName");

            return View();
        }

        // POST: CcgAppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Create(CreateCcgAppUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var ccgAppUser = Mapper.Map<CCGAppUser>(viewModel);

                unitOfWork.AppUserRepository.AddUser(ccgAppUser);

                // Get roles from EditRoles in VM and add to user
                _service.CreateUserRoles(ccgAppUser.Id, viewModel.EditRoles);

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }


        // GET: CcgAppUsers/Edit/5
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CCGAppUser ccgAppUser = unitOfWork.AppUserRepository.FindUserById(id);

            if (ccgAppUser == null)
            {
                return HttpNotFound();
            }

            var ccgs = unitOfWork.CCGRepository.FindAll();
            var ccgsVM = Mapper.Map<IList<CCGViewModel>>(ccgs);
            _ccgService.SetCCGViewName(ccgsVM);

            ViewBag.CCGs = new SelectList(ccgsVM, "Id", "CcgName", ccgAppUser.CcgId);
            var viewModel = Mapper.Map<EditCcgAppUserViewModel>(ccgAppUser);

            // Find roles and assign to EditRoles in view model
            viewModel.EditRoles = _service.SetViewModelEditRoles(ccgAppUser.Id);

            return View(viewModel);
        }

        // POST: CcgAppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Edit(EditCcgAppUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = unitOfWork.AppUserRepository.FindUserById(viewModel.Id);

                // update user properties
                _service.UpdateUserProperties(viewModel, user);

                // update user data
                unitOfWork.AppUserRepository.UpdateUser(user);

                // Get roles from EditRoles in VM and update user's roles
                _service.UpdateUserRoles(user.Id, viewModel.EditRoles);
                

                return Redirect(viewModel.ReturnUrl);
            }
            return View(viewModel);
        }

        // GET: CcgAppUsers/Delete/5
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCGAppUser ccgAppUser = unitOfWork.AppUserRepository.FindUserById(id);

            if (ccgAppUser == null)
            {
                return HttpNotFound();
            }

            return AutoMapView<DeleteCcgAppUserViewModel>(ccgAppUser, View());
        }

        // POST: CcgAppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult DeleteConfirmed(DeleteCcgAppUserViewModel viewModel)
        {
            CCGAppUser ccgAppUser = unitOfWork.AppUserRepository.FindUserById(viewModel.Id);
            unitOfWork.AppUserRepository.RemoveUser(ccgAppUser);

            return Redirect(viewModel.ReturnUrl);
        }
    }
}
