using System.Web;
using System.Web.Mvc;
using DeaconCCGManagement.BlobStorage;
using DeaconCCGManagement.Infrastructure;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class BlobController : ControllerBase
    {
        private BlobStorageReposity repo;

        public BlobController()
        {
            repo = new BlobStorageReposity();
        }

        
        public ActionResult Index()
        {   
            var blobVM = repo.GetBlobs();
            return View(blobVM);
        }

        [HttpGet]
        public ActionResult UploadBlob()
        {            
            return View();
        }

        [HttpPost]
        public ActionResult UploadBlob(HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null) return View();
            string newFileName = BlobService.GetNewFileName(uploadFile.FileName);
            uploadFile = Request.Files[0];


            bool hasUploaded = repo.UploadBlob(uploadFile, newFileName);
            if (hasUploaded) return RedirectToAction("Index");
            return View();
        }

        public ActionResult DownloadBlob(string file, string extension)
        {
            repo.DownloadBlobAsync(file + extension);
            return RedirectToAction("Index");
        }

        public JsonResult RemoveBlob(string file, string extension)
        {
            //todo get file name

            bool isDeleted = repo.DeleteBlob(file);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        } 

    }
}