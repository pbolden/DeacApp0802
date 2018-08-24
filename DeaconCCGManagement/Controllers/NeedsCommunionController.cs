using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Infrastructure;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using PagedList;
using Rotativa.MVC;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class NeedsCommunionController : ControllerBase
    {
        private NeedsCommunionService _service;

        public NeedsCommunionController()
        {
            _service = new NeedsCommunionService(base.unitOfWork);
        }

        // GET: NeedsCommunion
        [CCGAuthorize("Administrator", "Deacon Leadership")]
        public ActionResult Index(int? page=1, int? itemsPerPage=10, 
            int? dateRange = (int)CommunionDateRange.Last30Days,
            int? purgeOption = (int)PurgeNeedsCommunion.None)
        {
            ViewBag.ItemsPerPage = itemsPerPage;
            ViewBag.DateRange = dateRange;

            var purgeNeedsCommunion = purgeOption == null ? PurgeNeedsCommunion.None : (PurgeNeedsCommunion) purgeOption;
            if (purgeNeedsCommunion != PurgeNeedsCommunion.None)
            {
                _service.PurgeNeedsCommunionRecords(purgeNeedsCommunion);
            }

            var records = unitOfWork.NeedsCommunionRepository.FindAll().OrderByDescending(r => r.Timestamp).ToList();
            var dateRangeFilter = dateRange == null ? CommunionDateRange.Last30Days : (CommunionDateRange) dateRange;
            _service.FilterByDateRange(records, dateRangeFilter);

            var viewModel = new NeedsCommunionListViewModel();
            var needsCommunionList = records.Select(record => new NeedsCommunionViewModel
            {
                MemberId = record.MemberId,
                FirstName = record.Member.FirstName,
                LastName = record.Member.LastName,
                LastSelected = record.Timestamp,
            }).ToList();

            viewModel.Count = records.Count;
            viewModel.CommunionDateRange = dateRangeFilter;
            viewModel.PurgeNeedsCommunion = purgeNeedsCommunion;
            viewModel.MembersNeedingCommunion = needsCommunionList.ToPagedList(page ?? 1, itemsPerPage ?? 10);

            return View(viewModel);
        }


        public ActionResult DownloadAsPdf()
        {
            //fileName = GetFileNameWithDate(fileName);
            return new ActionAsPdf("Index") { FileName = "needs communion" };
        }
        
        // GET
        public ActionResult Edit(int? memberId)
        {
            if (memberId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var member = unitOfWork.MemberRepository.FindMemberById(memberId);
            if (member == null)
            {
                return HttpNotFound();
            }
           
            var needsCommunion = unitOfWork.NeedsCommunionRepository
                .Find(c => c.MemberId == memberId, firstOrDefault:true);
            var viewModel = new NeedsCommunionViewModel
            {
                MemberId = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                LastSelected = needsCommunion?.Timestamp,
                // User cannot select a member more than once per day
                SelectionAllowed = _service.IsSelectionAllowed(needsCommunion)
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NeedsCommunionViewModel viewModel)
        {
            if (!viewModel.NeedsCommunion) return Redirect(viewModel.ReturnUrl);
            var needsCommunion = new NeedsCommunion
            {
                MemberId = viewModel.MemberId,
                Timestamp = DateTime.Now
            };
            unitOfWork.NeedsCommunionRepository.Add(needsCommunion);
           
            return Redirect(viewModel.ReturnUrl);
        }
    }
}