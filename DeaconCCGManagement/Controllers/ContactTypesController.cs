using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class ContactTypesController : ControllerBase
    {
        // GET: ContactTypes
        public ActionResult Index()
        {
            var contactTypes = unitOfWork.ContactTypeRepository.FindAll().ToList();
            var viewModelList = contactTypes.Select(contactType => new ContactTypeViewModel
            {
                Id = contactType.Id, Name = contactType.Name
            }).ToList();

            return View(viewModelList);
        }

        // GET: ContactTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContactTypeViewModel viewModel)
        {
            //ContactType contactType = db.ContactTypeRepository.FindById(viewModel.Id);
            ContactType contactType = new ContactType
            {
                Name = viewModel.Name
            };

            if (ModelState.IsValid)
            {
                unitOfWork.ContactTypeRepository.Add(contactType);
                return Redirect(viewModel.ReturnUrl);
            }

            return View(viewModel);
        }

        // GET: ContactTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactType contactType = unitOfWork.ContactTypeRepository.FindById(id);
            if (contactType == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ContactTypeViewModel
            {
                Id = contactType.Id,
                Name = contactType.Name
            };
            return View(viewModel);
        }

        // POST: ContactTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContactTypeViewModel viewModel)
        {
            ContactType contactType = unitOfWork.ContactTypeRepository.FindById(viewModel.Id);
            contactType.Name = viewModel.Name;
            if (ModelState.IsValid)
            {
                unitOfWork.ContactTypeRepository.Update(contactType);
                
                return Redirect(viewModel.ReturnUrl);
            }
            return View(viewModel);
        }

        // GET: ContactRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactType contactType = unitOfWork.ContactTypeRepository.FindById(id);
            if (contactType == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var viewModel = new ContactTypeViewModel {Name = contactType.Name, Id = contactType.Id};
            return View(viewModel);
        }

        // POST: ContactRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(ContactTypeViewModel viewModel)
        {
            ContactType contactType = unitOfWork.ContactTypeRepository.FindById(viewModel.Id);
            unitOfWork.ContactTypeRepository.Delete(contactType);

            return Redirect(viewModel.ReturnUrl);
        }
    }
}
