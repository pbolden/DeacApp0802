using System.Collections.Generic;
using System.Linq;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using PagedList;

namespace DeaconCCGManagement.Services
{
    public class CcgAppUsersService : ServiceBase
    {
        private MemberPhotoService _photoService;

        public CcgAppUsersService(UnitOfWork uow) : base(uow)
        {
            _photoService = new MemberPhotoService(unitOfWork);
        }

        public void CreateUserRoles(string userId, EditRolesViewModel editRoles)
        {
            if (editRoles.IsAdmin)
            {
                unitOfWork.AppUserRepository.AddUserToRole(userId, AuthHelper.Admin);
            }
            if (editRoles.IsDeaconLeadership)
            {
                unitOfWork.AppUserRepository.AddUserToRole(userId, AuthHelper.DeaconLeadership);
            }
            if (editRoles.IsDeacon)
            {
                unitOfWork.AppUserRepository.AddUserToRole(userId, AuthHelper.Deacon);
            }
            if (editRoles.IsPastor)
            {
                unitOfWork.AppUserRepository.AddUserToRole(userId, AuthHelper.Pastor);
            }
        }

        public void SetHasPhotoFlag(IList<CcgAppUserViewModel> memberList)
        {
            foreach (var member in memberList)
            {
                member.HasPhoto = _photoService.DoesMemberHavePhoto(null, member.Id);
            }
        }

        public bool DoesUserHavePhoto(string userId)
        {
            return _photoService.DoesMemberHavePhoto(null, userId);
        }

        public void SetImgSrcAndHasPhotoFlag(IList<CcgAppUserViewModel> appUsers)
        {
            foreach (var user in appUsers)
                SetImgSrcAndHasPhotoFlag(user);

        }

        public void SetImgSrcAndHasPhotoFlag(AppUserViewModelBase appUser)
        {
            appUser.HasPhoto = !string.IsNullOrEmpty(appUser.PhotoFileName);
            if (appUser.HasPhoto)
                appUser.ImageSrc = _photoService.GetPhotoSrcString(appUser.PhotoFileName);
        }

        public void UpdateUserRoles(string userId, EditRolesViewModel editRoles)
        {
            UpdateUserRole(userId, AppUserRole.Admin, AuthHelper.Admin, editRoles.IsAdmin);
            UpdateUserRole(userId, AppUserRole.DeaconLeadership, AuthHelper.DeaconLeadership, editRoles.IsDeaconLeadership);
            UpdateUserRole(userId, AppUserRole.Deacon, AuthHelper.Deacon, editRoles.IsDeacon);
            UpdateUserRole(userId, AppUserRole.Pastor, AuthHelper.Pastor, editRoles.IsPastor);
        }

        private void UpdateUserRole(string userId, AppUserRole role, string roleName, bool inRole)
        {
            var user = unitOfWork.AppUserRepository.FindUserById(userId);
            if (inRole)
            {
                if (!unitOfWork.AppUserRepository.IsInRole(user.Email, role))
                {
                    unitOfWork.AppUserRepository.AddUserToRole(user.Id, roleName);
                }
            }
            else
            {
                if (unitOfWork.AppUserRepository.IsInRole(user.Email, role))
                {
                    unitOfWork.AppUserRepository.RemoveRoleFromUser(user.Id, roleName);
                }
            }
        }

        public EditRolesViewModel SetViewModelEditRoles(string userId)
        {
            var user = unitOfWork.AppUserRepository.FindUserById(userId);

            var editRoles = new EditRolesViewModel
            {
                IsAdmin = unitOfWork.AppUserRepository.IsInRole(user.Email, AppUserRole.Admin),
                IsDeaconLeadership = unitOfWork.AppUserRepository.IsInRole(user.Email, AppUserRole.DeaconLeadership),
                IsDeacon = unitOfWork.AppUserRepository.IsInRole(user.Email, AppUserRole.Deacon),
                IsPastor = unitOfWork.AppUserRepository.IsInRole(user.Email, AppUserRole.Pastor)
            };

            return editRoles;
        }

        public List<CCGAppUser> FilterAppUsers(List<CCGAppUser> users, AppUserFilter filter)
        {
            switch (filter)
            {
                case AppUserFilter.AllAppUsers:
                    return users;
                case AppUserFilter.Administrator:
                    return users.FindAll(u => AuthHelper.IsInRole(u.Email, AppUserRole.Admin));
                case AppUserFilter.DeaconLeadership:
                    return users.FindAll(u => AuthHelper.IsInRole(u.Email, AppUserRole.DeaconLeadership));
                case AppUserFilter.Deacon:
                    return users.FindAll(u => AuthHelper.IsInRole(u.Email, AppUserRole.Deacon));
                case AppUserFilter.Pastor:
                    return users.FindAll(u => AuthHelper.IsInRole(u.Email, AppUserRole.Pastor));
                case AppUserFilter.AdminAndDeacon:
                    return users.FindAll(u => AuthHelper.IsInRole(u.Email, AppUserRole.Admin)
                        && AuthHelper.IsInRole(u.Email, AppUserRole.DeaconLeadership));
                case AppUserFilter.LeadershipAndDeacon:
                    return users.FindAll(u => AuthHelper.IsInRole(u.Email, AppUserRole.DeaconLeadership)
                        && AuthHelper.IsInRole(u.Email, AppUserRole.Deacon));
                case AppUserFilter.ChangeRequestManager:
                    return users.FindAll(u => u.ChangeRequestManager);
                default:
                    return users;
            }
        }
        /// <summary>
        /// Change phone number to tel link for list of users
        /// eg. "tel:+15551234567"
        /// </summary>
        /// <param name="users"></param>
        public void SetHrefLinkForPhoneNumbers(IEnumerable<AppUserViewModelBase> users)
        {
            foreach (var user in users)
            {
                SetHrefLinkForPhoneNumbers(user);
            }
        }
        /// <summary>
        /// Change phone number to tel link for a single user
        /// eg. "tel:+15551234567"
        /// </summary>
        /// <param name="user"></param>
        public void SetHrefLinkForPhoneNumbers(AppUserViewModelBase user)
        {
            user.HrefPhoneNumberLink = PhoneCallService.GetHrefPhoneNumberLink(user.PhoneNumber);
            user.HrefCellNumberLink = PhoneCallService.GetHrefPhoneNumberLink(user.CellNumber);
        }
        /// <summary>
        /// Updates the user's properties with new data.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="user"></param>
        public void UpdateUserProperties(EditCcgAppUserViewModel viewModel, CCGAppUser user)
        {
            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            user.CcgId = viewModel.CcgId;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.CellNumber = viewModel.CellNumber;
            user.ChangeRequestManager = viewModel.ChangeRequestManager;
            user.EmailAddress = viewModel.EmailAddress;
            user.Email = viewModel.SharePointEmail;
        }

        public List<CCGAppUser> SearchUsers(string query)
        {
            return unitOfWork.AppUserRepository.FindUsers(u => u.FirstName.Contains(query) 
                || u.LastName.Contains(query)).ToList();
        }
    }
}