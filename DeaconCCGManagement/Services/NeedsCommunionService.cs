using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Models;
using WebGrease.Css.Extensions;

namespace DeaconCCGManagement.Services
{
    public class NeedsCommunionService : ServiceBase
    {
        public NeedsCommunionService(UnitOfWork uow) : base(uow)
        {

        }

        public bool IsSelectionAllowed(NeedsCommunion needsCommunion)
        {
            if (needsCommunion == null) return true;

            // if year or day != today's date then return true
            return needsCommunion.Timestamp.Year != DateTime.Today.Year
                   || needsCommunion.Timestamp.DayOfYear != DateTime.Today.DayOfYear;
        }

        public void FilterByDateRange(List<NeedsCommunion> records, CommunionDateRange dateRange)
        {
            if (records == null) return;

            TimeSpan timespan;
            switch (dateRange)
            {
                case CommunionDateRange.Last30Days:
                    timespan = TimeSpan.FromDays(30);
                    break;
                case CommunionDateRange.Last60Days:
                    timespan = TimeSpan.FromDays(60);
                    break;
                case CommunionDateRange.Last90Days:
                    timespan = TimeSpan.FromDays(90);
                    break;
                case CommunionDateRange.Last6Months:
                    timespan = TimeSpan.FromDays(180);
                    break;
                case CommunionDateRange.LastYear:
                    timespan = TimeSpan.FromDays(365);
                    break;
                case CommunionDateRange.None:
                default:
                    return;
            }
            var dateTimeOffset = DateTime.Now.Subtract(timespan);

            // Remove all that are older than the offset date
            // '<' means older. Eg, if the timestamp is 90 days 
            // ago and the offset date is 60 days ago, the 
            // contact will be removed. 3/15/17 > 3/15/16
            records.RemoveAll(r => r.Timestamp <= dateTimeOffset);
        }

        public void PurgeNeedsCommunionRecords(PurgeNeedsCommunion purgeOption)
        {
            TimeSpan timespan;
            switch (purgeOption)
            {
                case PurgeNeedsCommunion.Over60Days:
                    timespan = TimeSpan.FromDays(60);
                    break;
                case PurgeNeedsCommunion.Over6Months:
                    timespan = TimeSpan.FromDays(180);
                    break;
                case PurgeNeedsCommunion.Over1Year:
                    timespan = TimeSpan.FromDays(365);
                    break;
                default:
                    return;
            }
            var dateTimeOffset = DateTime.Now.Subtract(timespan);
            var records = unitOfWork.NeedsCommunionRepository.FindAll(r => r.Timestamp < dateTimeOffset);
            records.ForEach(r => unitOfWork.NeedsCommunionRepository.Delete(r));
        }
    }
}