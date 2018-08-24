using System.Configuration;
using DeaconCCGManagement.DAL;

namespace DeaconCCGManagement.Services
{
    public class AuthService
    {
        private readonly UnitOfWork unitOfWork;

        public AuthService(UnitOfWork unitOfWork)
        {
            unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Determines if off-line development configuration is on or off.
        /// </summary>
        /// <returns></returns>
        public static bool IsOfflineDevelopment()
        {
            var flag = ConfigurationManager.AppSettings["OfflineDevelopment"];
            bool offlineDevelopment;
            return bool.TryParse(flag, out offlineDevelopment) && offlineDevelopment;
        }

    }
}