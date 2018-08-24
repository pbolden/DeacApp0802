using System;
using System.Linq;
using System.Web.Mvc;
using Rotativa.Core;
using Rotativa.MVC;

namespace DeaconCCGManagement.Controllers
{
    public class PDFController : ControllerBase
    {
        public ActionResult DownloadUrlAsPdf(string url, string fileName)
        {
            fileName = GetFileNameWithDate(fileName);
            fileName = AddExt(fileName);

            // Get cookies from the request and pass them to Rotativa.
            // Necessary to make PDFs from action methods with Authorization attribute
            var cookies = Request.Cookies.AllKeys
                  .ToDictionary(key => key, key => Request.Cookies[key]?.Value);
            
            var options = new DriverOptions
            {
                Cookies = cookies
            };
          
            return new UrlAsPdf(url) { FileName = fileName, RotativaOptions = options };
        }

        public ActionResult DownloadChangeRequestAsPdf(int? id)
        {
            var changeRequest = unitOfWork.ChangeRequestRepository.FindById(id);
            var fileName = GetFileNameWithDate("Change-Request");
            fileName = $"{fileName}-{changeRequest.CcgMember.FirstName}-{changeRequest.CcgMember.LastName}";
            fileName = AddExt(fileName);
            return new ActionAsPdf("../ChangeRequests/ChangeRequest", new { id = id }) { FileName = fileName };
        }

        private string GetFileNameWithDate(string fileName)
        {
            return $"{fileName}-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
        }

        private string AddExt(string fileName)
        {
            return $"{fileName}.pdf";
        }
    }
}