using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using DeaconCCGManagement.Infrastructure;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using PagedList;

namespace DeaconCCGManagement.Controllers
{
    [CCGAuthorize("Administrator", "Deacon Leadership")]
    public class CCGController : ControllerBase
    {
        private readonly CCGService _service;

        public CCGController()
        {
            _service = new CCGService(unitOfWork);
        }

        // GET: CCG
        public ActionResult Index()
        {
            var ccgs = unitOfWork.CCGRepository.FindAll();
            var ccgsVM = Mapper.Map<IList<CCGViewModel>>(ccgs);

            // Concats CCG name with deacons last names
            _service.SetCCGViewName(ccgsVM);
            
            return View(ccgsVM);
        }

        // GET: CCG/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CCG/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CCGViewModel ccgViewModel)
        {
            if (ModelState.IsValid)
            {
                var ccg = new CCG
                {
                    CCGName = ccgViewModel.CCGName
                };
                unitOfWork.CCGRepository.Add(ccg);

                return RedirectToAction("Index");
            }

            return View(ccgViewModel);
        }

        // GET: CCG/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ccg = unitOfWork.CCGRepository.FindById(id);
            if (ccg == null)
            {
                return HttpNotFound();
            }

            return AutoMapView<CCGViewModel>(ccg, View());
        }

        // POST: CCG/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CCGViewModel ccgViewModel)
        {
            if (ModelState.IsValid)
            {
                var ccg = unitOfWork.CCGRepository.FindById(ccgViewModel.Id);
                ccg.CCGName = ccgViewModel.CCGName;
                unitOfWork.CCGRepository.Update(ccg);
                return RedirectToAction("Index");
            }
            return View(ccgViewModel);
        }

        // GET: CCG/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ccg = unitOfWork.CCGRepository.FindById(id);
            if (ccg == null)
            {
                return HttpNotFound();
            }

            var ccgVM = Mapper.Map<CCGViewModel>(ccg);
            _service.SetCCGViewName(ccgVM);

            return View(ccgVM);
        }

        // POST: CCG/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(CCGViewModel viewModel)
        {
            var ccg = unitOfWork.CCGRepository.FindById(viewModel.Id);
            unitOfWork.CCGRepository.Delete(ccg);
            return RedirectToAction("Index");
        }
    }
}
