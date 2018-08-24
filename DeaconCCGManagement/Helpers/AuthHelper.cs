using System.Linq;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.Services;
using DeaconCCGManagement.ViewModels;

namespace DeaconCCGManagement.Helpers
{
    /// <summary>
    /// Helpers for authorizing the user.
    /// </summary>
    public static class AuthHelper
    {
        #region Roles as strings
        public static string Admin => "Administrator";

        public static string DeaconLeadership => "Deacon Leadership";

        public static string Deacon => "Deacon";

        public static string Pastor => "Pastor";
        #endregion


        public static bool IsInRole(string email, AppUserRole role)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.AppUserRepository.IsInRole(email, role);
            }
        }

        public static bool IsInRole(string email, AppUserRole[] roles)
        {
            return roles.Any(role => IsInRole(email, role));
        }

        public static bool HasAdminAccess(string email)
        {
            return IsInRole(email, AppUserRole.Admin) ||
                   IsInRole(email, AppUserRole.DeaconLeadership);
        }

        public static UserLoggedInInfoViewModel GetUserLoggedInInfo(string userEmail)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);

                if (user != null)
                {
                    return new UserLoggedInInfoViewModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        FirstInitialLastName = $"{user.FirstName.Substring(0, 1)}. {user.LastName}",
                        HasPhoto = !string.IsNullOrEmpty(user.PhotoFileName),
                        ImageSrc = !string.IsNullOrEmpty(user.PhotoFileName) ?
                            new MemberPhotoService(unitOfWork).GetPhotoSrcString(user.PhotoFileName) : string.Empty
                    };
                }
                return new UserLoggedInInfoViewModel();
            }

        }

        public static string GetUserFullName(string userEmail)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
                return $"{user.FullName}";
            }

        }

        public static string GetUserId(string userEmail)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
                return user.Id;
            }

        }

        public static string GetUserSurname(string userEmail)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
                return $"{user.LastName}";
            }

        }

        public static int? GetUserCcgId(string userEmail)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
                return user.CcgId;
            }

        }

        public static bool IsChangeRequestManager(string userEmail)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
                return user != null && user.ChangeRequestManager;
            }

        }

        /// <summary>
        /// Checks if user is the member's deacon.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static bool IsMemberDeacon(string userEmail, int memberId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return IsMemberDeaconHelper(userEmail, memberId, unitOfWork);
            }
        }

        /// <summary>
        /// Checks if user is the member's deacon with the option to pass UnitOfWork object.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="memberId"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public static bool IsMemberDeacon(string userEmail, int memberId, UnitOfWork unitOfWork)
        {
            return IsMemberDeaconHelper(userEmail, memberId, unitOfWork);
        }
        /// <summary>
        /// Checks if user is the member's deacon by passing user and member's CCG names.
        /// </summary>
        /// <param name="userCCGName"></param>
        /// <param name="memberCCGName"></param>
        /// <returns></returns>
        public static bool IsMemberDeacon(string userCCGName, string memberCCGName)
        {
            return userCCGName != null && memberCCGName != null && userCCGName.Equals(memberCCGName);
        }

        private static bool IsMemberDeaconHelper(string userEmail, int memberId, UnitOfWork unitOfWork)
        {
            var member = unitOfWork.MemberRepository.FindMemberById(memberId);
            var user = unitOfWork.AppUserRepository.FindUserByEmail(userEmail);
            return user.CCG != null && member.CCG != null
                && user.CCG.CCGName.Equals(member.CCG.CCGName);
        }
    }
}