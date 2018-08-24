using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Security.AntiXss;
using DeaconCCGManagement.Controllers;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;

namespace DeaconCCGManagement.Services
{
    public class ContactRecordsService : ServiceBase
    {
        // used to remove prayer requests
        readonly string _prayerRequestName = "Prayer Request";

        public ContactRecordsService(UnitOfWork uow) : base(uow)
        {
        }

        public IEnumerable<ContactRecord> GetContactRecords(int? memberId, int? ccgId, bool getAll, 
            bool archive, CCGAppUser user, DateRangeFilter dateRangeFilter, 
            string query, out ActionResult actionResult)
        {
            actionResult = null;
            DateTime dateTimeOffset;
            GetOffsetDate(dateRangeFilter, out dateTimeOffset);

            // Prevents having one query for search and one for no search.
            if (query == null) query = "";

            // Get contact records for specific member
            if (memberId != null)
            {        
                return unitOfWork.ContactRecordRepository
                        .GetContactRecords(c => c.CCGMemberId == memberId
                        && c.Archive == archive
                        && c.Timestamp > dateTimeOffset
                        && c.ContactType.Name != _prayerRequestName
                        && (c.Subject.Contains(query) || c.Comments.Contains(query))).ToList();
            }
            // Get all contact records for a CCG passed to parameter
            if (ccgId != null && ccgId != -1)
            {
                return unitOfWork.ContactRecordRepository
                    .GetContactRecords(c => c.CCGMember.CcgId == ccgId 
                    && c.Archive == archive 
                    && c.Timestamp > dateTimeOffset
                    && c.ContactType.Name != _prayerRequestName
                    && (c.Subject.Contains(query) || c.Comments.Contains(query))).ToList();
            }
            // Get all contact records; admin or leadership only
            if (getAll)
            {
                if (!CanViewAllRecords(user.Email))
                {
                    // If denied permission, get all records for user's CCG only
                    return GetContactRecordsForCcg(archive, user, dateTimeOffset, 
                       query, out actionResult).ToList();
                }
                
                // Gets all contact records. 
                return unitOfWork.ContactRecordRepository
                   .GetContactRecords(c => c.Archive == archive 
                   && c.Timestamp > dateTimeOffset
                   && c.ContactType.Name != _prayerRequestName
                   && (c.Subject.Contains(query) || c.Comments.Contains(query))).ToList();
            }

            // Get all contact records for the user's CCG only
            return GetContactRecordsForCcg(archive, user, dateTimeOffset, query, 
                out actionResult).ToList();
        }

        private IEnumerable<ContactRecord> GetContactRecordsForCcg(bool archive,
            CCGAppUser user, DateTime dateTimeOffset, string query,
            out ActionResult actionResult)
        {
            actionResult = null;

            // find the user's assigned CCG
            var ccg = unitOfWork.CCGRepository.Find(c => c.Id == user.CcgId);

            if (ccg == null)
            {
                actionResult = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return null;
            }
           
            // find contact records of members in the user's CCG
            return unitOfWork.ContactRecordRepository.GetContactRecords(
                c => c.CCGMember.CcgId == ccg.Id 
                && c.Archive == archive
                && c.Timestamp > dateTimeOffset
                && c.ContactType.Name != _prayerRequestName
                && (c.Subject.Contains(query) || c.Comments.Contains(query))).ToList();

        }
        /// <summary>
        /// Authorize user to view all contact records.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool CanViewAllRecords(string userEmail)
        {
            var roles = new[]
              {
                    AppUserRole.Admin,
                    AppUserRole.DeaconLeadership,
                    AppUserRole.Pastor,
              };
            return AuthHelper.IsInRole(userEmail, roles);
        }
        /// <summary>
        /// Sort contact records.
        /// </summary>
        /// <param name="sortOption"></param>
        /// <param name="contactRecords"></param>
        /// <returns></returns>
        public IEnumerable<ContactRecord> SortContactRecords(ContactsSort sortOption, IEnumerable<ContactRecord> contactRecords)
        {
            var contactRecordsList = contactRecords as List<ContactRecord>;
            if (contactRecordsList == null) return contactRecords;

            switch (sortOption)
            {
                case ContactsSort.GroupByMember:
                    return contactRecordsList.GroupBy(c => c.CCGMemberId)
                        .SelectMany(@group => @group)
                        .OrderBy(r => r.CCGMember.LastName)
                        .ThenByDescending(r => r.ContactDate);
                case ContactsSort.GroupByDeacon:
                    return contactRecordsList.GroupBy(c => c.AppUserId)
                        .SelectMany(@group => @group)
                        .OrderBy(r => r.AppUser.LastName)
                        .ThenByDescending(r => r.ContactDate);
                case ContactsSort.GroupByContactType:
                    return contactRecordsList.GroupBy(c => c.ContactTypeId)
                        .SelectMany(@group => @group);
                case ContactsSort.DateAscending:
                    return contactRecordsList.OrderBy(c => c.ContactDate);
                case ContactsSort.DateDescending:
                case ContactsSort.None:
                    return contactRecordsList.OrderByDescending(c => c.ContactDate);
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortOption), sortOption, null);
            }
        }
        /// <summary>
        /// Remove all prayer requests since they have their own views
        /// </summary>
        /// <param name="contactRecords"></param>
        /// <returns></returns>
        public IEnumerable<ContactRecord> RemovePrayerRequests(IEnumerable<ContactRecord> contactRecords)
        {
            var contactRecordsList = contactRecords as List<ContactRecord>;

            if (contactRecordsList != null)
            {
                contactRecordsList.RemoveAll(
                    r => r.ContactType.Name
                        .Equals("Prayer Request", StringComparison.CurrentCultureIgnoreCase));

                return contactRecordsList;
            }
            return contactRecords;
        }
        /// <summary>
        /// Search contact records by query entered by user.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="contactRecords"></param>
        /// <returns></returns>
        public IEnumerable<ContactRecord> SearchContactRecords(string query, IEnumerable<ContactRecord> contactRecords)
        {
            if (query == null) return contactRecords;
            var contactRecordsList = contactRecords as List<ContactRecord>;
            if (contactRecordsList != null)
                return contactRecordsList.FindAll(r => r.Subject.Contains(query)
                                                       || r.Comments.Contains(query));
            return contactRecords;
        }

        /// <summary>
        /// Truncates the subjects and comments. eg. "I wished her Merry..."
        /// </summary>
        /// <param name="contactRecords">Collection of contact records to truncate.</param>
        public void TruncateText(IEnumerable<ContactRecordViewModel> contactRecords)
        {
            const int commentsMaxLength = 20;
            const int subjectMaxLength = 30;
            var sb = new StringBuilder();
            foreach (var contactRecord in contactRecords)
            {
                TruncateComment(contactRecord, commentsMaxLength, sb);
                TruncateSubject(contactRecord, subjectMaxLength, sb);
            }
        }

        /// <summary>
        /// Truncates subjects, comments, and message to pastor; 
        /// also replaces comments if marked as private
        /// </summary>
        /// <param name="contactRecords"></param>
        /// <param name="userId"></param>
        public void TruncateTextAndKeepPrivate(IEnumerable<ContactRecordViewModel> contactRecords, string userId)
        {
            var sb = new StringBuilder();
            foreach (var contactRecord in contactRecords)
            {
                TruncateComment(contactRecord, 30, sb);
                TruncateSubject(contactRecord, 30, sb);
                TruncatePassAlongMessage(contactRecord, 50, sb);
                KeepCommentsPrivate(contactRecord, userId);
            }
        }

        /// <summary>
        /// Checks if comments are marked as private by deacon. If
        /// so, replaces comments with the word "Private" for other users.
        /// </summary>
        /// <param name="contactRecords">Collection of contact records</param>
        /// <param name="userId">If of user currently logged in</param>
        public void KeepCommentsPrivate(IEnumerable<ContactRecordViewModel> contactRecords, string userId)
        {
            foreach (var contactRecord in contactRecords)
            {
                KeepCommentsPrivate(contactRecord, userId);
            }
        }

        /// <summary>
        /// Checks if comments are marked as private by deacon. If
        /// so, replaces comments with the word "Private" for other users.
        /// </summary>
        /// <param name="contactRecord"></param>
        /// <param name="userId"></param>
        public void KeepCommentsPrivate(ContactRecordViewModel contactRecord, string userId)
        {
            const string privateText = "Private";

            if (!contactRecord.Private) return;

            // Only keep private for other users
            if (contactRecord.AppUserId != userId)
            {
                contactRecord.Comments = privateText;
            }
        }

        public void TruncateComment(ContactRecordViewModel contactRecord,
            int maxCharLength, StringBuilder sb)
        {
            if (contactRecord.Comments == null ||
                contactRecord.Comments.Length <= maxCharLength) return;

            sb.Append(contactRecord.Comments.Substring(0, maxCharLength));
            sb.Append("...");
            contactRecord.Comments = sb.ToString();
            sb.Clear();
        }

        public void TruncateSubject(ContactRecordViewModel contactRecord,
            int maxCharLength, StringBuilder sb)
        {
            if (contactRecord.Subject == null ||
                contactRecord.Subject.Length <= maxCharLength) return;

            sb.Append(contactRecord.Subject.Substring(0, maxCharLength));
            sb.Append("...");
            contactRecord.Subject = sb.ToString();
            sb.Clear();
        }
        /// <summary>
        /// Truncates pass along message for the list view. 
        /// eg. Pastor, this person needs... 
        /// </summary>
        /// <param name="contactRecords"></param>
        public void TruncatePassAlongMessages(IEnumerable<ContactRecordViewModel> contactRecords)
        {
            const int maxCharLength = 30;
            var sb = new StringBuilder();
            foreach (var contactRecord in contactRecords)
            {
                TruncatePassAlongMessage(contactRecord, maxCharLength, sb);
            }
        }

        private void TruncatePassAlongMessage(ContactRecordViewModel contactRecord,
            int maxCharLength, StringBuilder sb)
        {
            // If pass along message is null or <= max char length
            if (contactRecord.PassAlongComments == null ||
            contactRecord.PassAlongComments.Length <= maxCharLength) return;

            sb.Append(contactRecord.PassAlongComments.Substring(0, maxCharLength));
            sb.Append("...");
            contactRecord.PassAlongComments = sb.ToString();
            sb.Clear();
        }

        /// <summary>
        /// Updates the contact record pulled from the database
        /// with the edited contact record
        /// </summary>
        /// <param name="contactRecord"></param>
        /// <param name="editedContactRecord"></param>
        /// /// <param name="db"></param>
        public void UpdateContactRecord(ContactRecord contactRecord,
                                        ContactRecord editedContactRecord)
        {
            contactRecord.ContactDate = editedContactRecord.ContactDate;
            contactRecord.Duration = editedContactRecord.Duration;
            contactRecord.PassAlong = editedContactRecord.PassAlong;
            contactRecord.PassAlongComments = editedContactRecord.PassAlongComments;
            contactRecord.Subject = editedContactRecord.Subject;
            contactRecord.Comments = editedContactRecord.Comments;
            contactRecord.ContactTypeId = editedContactRecord.ContactTypeId;
            contactRecord.ContactType = editedContactRecord.ContactType;
            contactRecord.Private = editedContactRecord.Private;

            unitOfWork.ContactRecordRepository.Update(contactRecord);
        }

        public void FilterContactsByDateRange(List<ContactRecord> records, DateRangeFilter dateRangeFilter)
        {
            if (records == null) return;

            DateTime dateTimeOffset;
            GetOffsetDate(dateRangeFilter, out dateTimeOffset);

            // Remove all that are older than the offset date
            // '<' means older. Eg, if the timestamp is 90 days 
            // ago and the offset date is 60 days ago, the 
            // contact will be removed. 3/15/17 > 3/15/16
            records.RemoveAll(cr => cr.Timestamp <= dateTimeOffset);
        }

        public void GetOffsetDate(DateRangeFilter dateRangeFilter, out DateTime dateTimeOffset)
        {
            TimeSpan timespan = new TimeSpan(0,0,0);
            switch (dateRangeFilter)
            {
                case DateRangeFilter.TwoWeeks:
                    timespan = TimeSpan.FromDays(14);
                    break;
                case DateRangeFilter.LastMonth:
                    timespan = TimeSpan.FromDays(30);
                    break;
                case DateRangeFilter.LastTwoMonths:
                    timespan = TimeSpan.FromDays(60);
                    break;
                case DateRangeFilter.ThreeMonths:
                    timespan = TimeSpan.FromDays(90);
                    break;
                case DateRangeFilter.SixMonths:
                    timespan = TimeSpan.FromDays(180);
                    break;
                case DateRangeFilter.LastYear:
                    timespan = TimeSpan.FromDays(365);
                    break;
                case DateRangeFilter.None:
                    dateTimeOffset = new DateTime(2000, 1, 1);
                    return;
            }
            dateTimeOffset = DateTime.Now.Subtract(timespan);
        }

        public ContactRecordViewModel SanitizeContactRecordViewModel(ContactRecordViewModel viewModel)
        {   
            viewModel.Comments = AntiXssEncoder.HtmlEncode(viewModel.Comments, false);
            viewModel.PassAlongComments = AntiXssEncoder.HtmlEncode(viewModel.PassAlongComments, false);
            viewModel.Subject = AntiXssEncoder.HtmlEncode(viewModel.Subject, false);
            viewModel.PassAlongFollowUpComments = AntiXssEncoder.HtmlEncode(viewModel.PassAlongFollowUpComments, false);
            return viewModel;
        }
    }
}