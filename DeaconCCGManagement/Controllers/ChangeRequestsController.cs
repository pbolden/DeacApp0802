using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using PagedList;

namespace DeaconCCGManagement.Controllers
{
    public class ChangeRequestsController : ControllerBase
    {
        private readonly CcgMembersService _service;

        public ChangeRequestsController()
        {
            _service = new CcgMembersService(base.unitOfWork);
        }

        // GET: ChangeRequests
        public ActionResult Index(int? page= 1, int? itemsPerPage = 10)
        {
            ViewBag.ItemsPerPage = itemsPerPage;
           
            var changeRequests = unitOfWork.ChangeRequestRepository.GetChangeRequests();
            var changeRequestList = new List<ChangeRequestViewModel>();

            foreach (var cr in changeRequests)
            {
                changeRequestList.Add(new ChangeRequestViewModel
                {
                    Id = cr.Id,
                    CRDate = cr.CRDate,
                    AppUserFrom = _service.GetChangeRequestAppUser(cr.Deacon),
                    CurrentMemberData = Mapper.Map<EditMemberViewModel>(cr.CcgMember)
                });
            }
            return View(changeRequestList.ToPagedList(page ?? 1, itemsPerPage ?? 10));         
            
        }
       
        public ActionResult ChangeRequest(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // user(s) assigned to handle change requests 
            var crManagers = unitOfWork.AppUserRepository.FindUsers(u => u.ChangeRequestManager).ToList();

            // converts CR managers to list of ChangeRequestViewModel.AppUser
            var changeRequestManagers = crManagers
                .Select(appUser => _service.GetChangeRequestAppUser(appUser)).ToList();

            var changeRequest = unitOfWork.ChangeRequestRepository.FindById(id);

            if (changeRequest == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var member = unitOfWork.MemberRepository.FindMemberById(changeRequest.CcgMemberId);
            var editMemberVM = Mapper.Map<EditMemberViewModel>(member);

            var changeRequestVM = new ChangeRequestViewModel
            {
                CRDate = changeRequest.CRDate,
                AppUserFrom = _service.GetChangeRequestAppUser(changeRequest.Deacon),
                AppUsersTo = changeRequestManagers,
                CurrentMemberData = editMemberVM,
                NewMemberData = Mapper.Map<EditMemberViewModel>(changeRequest)
            };


            return View(changeRequestVM);
        }    
       

        // GET: ChangeRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChangeRequest changeRequest = unitOfWork.ChangeRequestRepository.FindById(id);
            if (changeRequest == null)
            {
                return HttpNotFound();
            }
            return View(changeRequest);
        }

        // POST: ChangeRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChangeRequest changeRequest = unitOfWork.ChangeRequestRepository.FindById(id);
            unitOfWork.ChangeRequestRepository.Delete(changeRequest);

            return RedirectToAction("Index");
        }
    }
}
