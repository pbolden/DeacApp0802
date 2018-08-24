using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using DeaconCCGManagement.Comparers;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;

namespace DeaconCCGManagement.Services
{
    public class CcgMembersService : ServiceBase
    {
        private MemberPhotoService _photoService;

        public CcgMembersService(UnitOfWork unitOfWork) : base(unitOfWork)
        {
            _photoService = new MemberPhotoService(unitOfWork);
        }

        public IEnumerable<CCGMember> GetMembersList(int? ccgId, bool getAll, 
            string query, bool allAccess, string userEmail, out ActionResult actionResult)
        {
            IEnumerable<CCGMember> members = new List<CCGMember>();
            actionResult = null;

            // All access, eg. admin or pastor
            if (getAll) 
            {
                // User must be in approved role to view all members
                if (!CanViewAllMembers(userEmail))
                {
                    //actionResult = RedirectToAction("Index", "Home");
                    return members;
                }
                members = GetAllMembers();
            }
            else if (query != null) // Search
            {
                members = GetQueryResult(ref ccgId, query, allAccess, userEmail);
                //ViewBag.Query = query;
            }
            else if (ccgId != null) // Members in a CCG
            {
                // User must be in approved role 
                if (!CanViewAllMembers(userEmail))
                {
                    //actionResult = RedirectToAction("Index", "Home");
                    return members;
                }
                members = unitOfWork.MemberRepository.FindMembers(m => m.CcgId == ccgId);
            }
            else // Deacon accessing members in CCG
            {
                members = GetMembers(userEmail) ?? new List<CCGMember>();
            }
            return members;
        }
     

        public void SetImgSrcAndHasPhotoFlag(IList<ListMembersViewModel> memberList)
        {
            foreach (var member in memberList)
                SetImgSrcAndHasPhotoFlag(member);

        }

        public void SetImgSrcAndHasPhotoFlag(MemberViewModelBase member)
        {
            member.HasPhoto = !string.IsNullOrEmpty(member.PhotoFileName);
            if (member.HasPhoto)
                member.ImageSrc = _photoService.GetPhotoSrcString(member.PhotoFileName);
        }


        public void SetIsMemberDeaconFlag(IList<ListMembersViewModel> memberList, CCGAppUser user)
        {
            foreach (var member in memberList)
            {
                member.IsUserMemberDeacon = user.CCG != null && AuthHelper.IsMemberDeacon(user.CCG.CCGName, member.CCG.CCGName);
                if (!member.IsUserMemberDeacon) SetPhoneHrefLinks(member);
            }
        }

        private static void SetPhoneHrefLinks(MemberViewModelBase member)
        {
            member.HrefPhoneNumberLink = PhoneCallService.GetHrefPhoneNumberLink(member.PhoneNumber);
            member.HrefCellNumberLink = PhoneCallService.GetHrefPhoneNumberLink(member.CellPhoneNumber);
        }

        public void FilterMembers(ref List<CCGMember> members, MemberFilter filter, CCGAppUser user=null)
        {
            if (members == null) return;

            var timespan = TimeSpan.Zero;
            switch (filter)
            {
                case MemberFilter.NoPhoto:
                {
                    members.RemoveAll(m => _photoService.DoesMemberHavePhoto(m.Id));
                    return;
                }
                case MemberFilter.ActiveMember:
                {
                    members.RemoveAll(m => !m.IsMemberActive);
                    return;
                }
                case MemberFilter.InactiveMember:
                {
                    members.RemoveAll(m => m.IsMemberActive);
                    return;
                }
                case MemberFilter.NeedsCommunion:
                {
                    // Get all needs communion records
                    var needsCommunionService = new NeedsCommunionService(unitOfWork);
                    var records = unitOfWork.NeedsCommunionRepository
                        .FindAll(r => r.Member.CcgId == user.CcgId)
                        .OrderByDescending(r => r.Timestamp)
                        .ToList();

                    // Filter by last 30 days
                    needsCommunionService.FilterByDateRange(records, CommunionDateRange.Last30Days);

                    // Filtering existing list is convoluted. Easier to wipe out list and re-populate.
                    members.Clear();
                    if (user == null) return;
                    members.AddRange(records.Select(record => unitOfWork.MemberRepository.FindMemberById((record.MemberId))));

                    // nix duplicates
                    var recordsHashSet = new HashSet<CCGMember>(members, new CCGMemberEqComparer());
                    members = recordsHashSet.ToList();
                   
                    return;
                }
                case MemberFilter.NoContactForOneMonth:
                {
                    timespan = TimeSpan.FromDays(30);
                    break;
                }
                case MemberFilter.NoContactForTwoMonths:
                {
                    timespan = TimeSpan.FromDays(60);
                    break;
                }
                case MemberFilter.NoContactForThreeMonths:
                {
                    timespan = TimeSpan.FromDays(90);
                    break;
                }
                case MemberFilter.NoContactForSixMonths:
                {
                    timespan = TimeSpan.FromDays(180);
                    break;
                }
                case MemberFilter.None:
                {
                    return;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
                }
            }

            if (timespan != TimeSpan.Zero)
            {
                var dateTimeOffset = DateTime.Now.Subtract(timespan);

                // Remove all that are older than the offset date
                // '>' means more recent. Eg, if the 'last contact date'  
                // is 15 days ago and the offset date is 30 days ago, 
                // the contact will be removed. 3/15/17 > 3/15/16
                members.RemoveAll(cr => cr.LastDateContacted >= dateTimeOffset);
            }
        }

        private IEnumerable<CCGMember> GetQueryResult(ref int? ccgId, string query, bool allAccess, string userEmail)
        {
            IEnumerable<CCGMember> members;

            // If querying all members and has permission
            if (allAccess && CanViewAllMembers(userEmail))
            {
                members = Search(query);
            }
            else // Query members in CCG only
            {
                ccgId = GetCCGId(ccgId, userEmail);
                members = Search(query, ccgId);
            }

            return members;
        }

        public IEnumerable<ListMembersViewModel> SelectAll(IEnumerable<ListMembersViewModel> members)
        {
            var listMembers = members as IList<ListMembersViewModel> ?? members.ToList();
            foreach (var member in listMembers)
            {
                member.Selected = true;
            }

            return listMembers;
        }

        /// <summary>
        /// Returns only members in a deacon's CCG if user in deacon role. 
        /// Otherwise returns all members
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public IEnumerable<CCGMember> GetMembers(string userEmail)
        {
            if (unitOfWork.AppUserRepository.IsInRole(userEmail, AuthHelper.Deacon))
            {
                // Find only members within the Deacon's CCG.
                var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
                return unitOfWork.MemberRepository.FindMembers(m => m.CcgId == user.CcgId).OrderBy(m => m.LastName);
            }
            // Otherwise return null.
            return null;
        }

        /// <summary>
        /// Gets all members.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private IEnumerable<CCGMember> GetAllMembers()
        {
            return unitOfWork.MemberRepository.FindMembers().OrderBy(m => m.LastName).ToList();
        }

        /// <summary>
        /// Gets all members from data store where id's in view model match.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public IEnumerable<CCGMember> GetMembersInViewModel(IList<ListMembersViewModel> viewModel)
        {
            return viewModel?.Select(member => 
                unitOfWork.MemberRepository.FindById(member.Id)).Where(result => result != null).ToList();
        }

        public IEnumerable<CCGMember> GetSelecteMembers(int?[] memberIds)
        {
            return memberIds.Select(memberId => unitOfWork.MemberRepository.FindMemberById(memberId)).ToList();
        }

        public IEnumerable<CCGMember> Search(string query, int? ccgId = null)
        {
            // Search all members is ccgId is null

            var result = new List<CCGMember>();
            // Search first and last name for query match.
            if (ccgId != null) // search members in CCG
            {
                result = unitOfWork.MemberRepository.FindMembers(m => m.CcgId == ccgId && (m.LastName.Contains(query) || m.FirstName.Contains(query))).OrderBy(m => m.LastName).ToList();
            }
            else // search all members
            {
                result = unitOfWork.MemberRepository.FindMembers(m => m.LastName.Contains(query) || m.FirstName.Contains(query)).OrderBy(m => m.LastName).ToList();
            }

            return result;
        }

        public bool CanViewAllMembers(string userEmail)
        {
            var roles = new[]
            {
                AppUserRole.Admin, AppUserRole.DeaconLeadership, AppUserRole.Pastor,
            };
            return AuthHelper.IsInRole(userEmail, roles);
        }

        public int? GetCCGId(int? ccgId, string userEmail)
        {
            if (ccgId != null) return ccgId;
            var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
            return user.CcgId;
        }

        /// <summary>
        /// Updates the members information.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public void UpdateMemberInfo(EditMemberViewModel viewModel)
        {
            var member = unitOfWork.MemberRepository.FindMemberById(viewModel.Id);
            MapEditMemberVMToMember(viewModel, member);
            unitOfWork.MemberRepository.Update(member);
        }

        /// <summary>
        /// Maps EditMemberViewModel to CCGMember.
        /// Manual mapping is safer than auto mapping for updates.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        private void MapEditMemberVMToMember(EditMemberViewModel viewModel, CCGMember member)
        {
            member.LastName = viewModel.LastName;
            member.FirstName = viewModel.FirstName;
            member.Title = viewModel.Title;
            member.Suffix = viewModel.Suffix;
            member.Address = viewModel.Address;
            member.City = viewModel.City;
            member.State = viewModel.State;
            member.ZipCode = viewModel.ZipCode;
            member.PhoneNumber = viewModel.PhoneNumber;
            member.CellPhoneNumber = viewModel.CellPhoneNumber;
            member.BirthDate = viewModel.BirthDate;
            member.EmailAddress = viewModel.EmailAddress;
            member.DateJoinedZion = viewModel.DateJoinedZion;
            member.AnniversaryDate = viewModel.AnniversaryDate;
            member.CcgId = viewModel.CcgId;
            member.IsMemberActive = viewModel.IsMemberActive;
        }

        public ChangeRequestViewModel.AppUser GetChangeRequestAppUser(CCGAppUser appUser)
        {
            return new ChangeRequestViewModel.AppUser
            {
                Id = appUser.Id, FullName = appUser.FullName,
            };
        }

        public void ValidatePhoneNumbers(IList<ListMembersViewModel> members)
        {
            foreach (var member in members)
            {
                member.PhoneNumber = ValidatePhoneNumber(member.PhoneNumber);
                member.CellPhoneNumber = ValidatePhoneNumber(member.CellPhoneNumber);
            }
        }

        /// <summary>
        /// Validates phone number. If it fails, returns empty string
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ValidatePhoneNumber(string number)
        {
            // RegEx for phone number validation
            string phone = @"(?x)
            ( \d{3}[-\s] | \(\d{3}\)\s? )
            \d{3}[-\s]?
            \d{4}";
            string phoneNoAreaCode = @"\d{3}-\d{4}"; //eg. 781-4321

            if (number == null) return string.Empty;
            return Regex.IsMatch(number, phone) 
                || Regex.IsMatch(number, phoneNoAreaCode) 
                ? number : string.Empty;
        }
    }
}