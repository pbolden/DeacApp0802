using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;

namespace DeaconCCGManagement.Services
{
    public class CCGService : ServiceBase
    {
        public CCGService(UnitOfWork uow) : base(uow)
        {
            unitOfWork = uow;
        }
        private string ConcatCCGName(string ccgName, string deacon, string oriCcgName)
        {
            string sepChar = ccgName.Equals(oriCcgName) ? " " : "_";
            ccgName += sepChar + deacon;
            return ccgName;
        }
        /// <summary>
        /// Concatenates ccg name with deacons last names.
        /// eg., CCG10 --> CCG10_Bolden_Jones
        /// </summary>
        /// <param name="ccg"></param>
        public string SetCCGViewName(CCGViewModel ccg)
        {
            string oriCcgName = ccg.CCGName;
            foreach (var deacon in ccg.AppUsers)
            {
                ccg.CCGName = ConcatCCGName(ccg.CCGName, deacon.LastName, oriCcgName);
            }

            return ccg.CCGName;
        }
        /// <summary>
        /// Concatenates ccg name with deacons last names.
        /// eg., CCG10 --> CCG10_Bolden_Jones
        /// </summary>
        /// <param name="ccg"></param>
        public string SetCCGViewName(CCG ccg)
        {
            string oriCcgName = ccg.CCGName;

            // To include app users. ccg param does not include them.
            ccg = unitOfWork.CCGRepository.FindById(ccg.Id);
           
            foreach (var deacon in ccg.AppUsers)
            {
                ccg.CCGName = ConcatCCGName(ccg.CCGName, deacon.LastName, oriCcgName);
            }

            return ccg.CCGName;
        }
        /// <summary>
        /// Concatenates ccg names with deacons last names.
        /// eg., CCG10 --> CCG10_Bolden_Jones
        /// </summary>
        /// <param name="ccgs"></param>
        public void SetCCGViewName(IEnumerable<CCG> ccgs)
        {
            foreach (var ccg in ccgs)
            {
                ccg.CCGName = SetCCGViewName(ccg);
            }
        }
        /// <summary>
        /// Concatenates ccg names with deacons last names.
        /// eg., CCG10 --> CCG10_Bolden_Jones
        /// </summary>
        /// <param name="ccgs"></param>
        public void SetCCGViewName(IEnumerable<CCGViewModel> ccgs)
        {
            foreach (var ccg in ccgs)
            {
                ccg.CCGName = SetCCGViewName(ccg);
            }
        }
    }
}