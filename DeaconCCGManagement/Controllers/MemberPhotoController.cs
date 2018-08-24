using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Services;
using MvcFileUploader.Models;

namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class MemberPhotoController : ControllerBase
    {
        private readonly MemberPhotoService _service;

        public MemberPhotoController()
        {
            _service = new MemberPhotoService(base.unitOfWork);
        }
        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public JsonResult UploadFile(string memberName, int? memberId=null, string userId=null)
        {
            string errMsg = "Sorry, file upload has failed. Please try again.";

            // For sending data back to the view
            var status = new ViewDataUploadFileResult();

            // Null and file length check
            if (Request.Files?[0] == null ||
                Request.Files[0].ContentLength == 0)
            {
                // Return error
                status.error = errMsg;
            }
            else
            {
                // Gets data from the posted image file and stores
                // the image in the database as a byte array
                _service.StoreImage(Request.Files[0], memberId, userId);

                // Populate status object to send back to view
                status.name = memberName;
                status.type = Request.Files[0].ContentType;
                status.url = ""; // url for name after upload not needed
                status.thumbnailUrl = GetThumbnailImgSrc(memberId, userId);
            }

            // MVCFileUpload widget expects a collection of file results
            var statuses = new List<ViewDataUploadFileResult>();
            statuses.Add(status);
            var viewresult = Json(new { files = statuses });

            //for IE8 which does not accept application/json
            if (Request.Headers["Accept"] != null && !Request.Headers["Accept"].Contains("application/json"))
                viewresult.ContentType = "text/plain";

            return viewresult;
        }
        /// <summary>
        /// Deprecated since we're using blob storage.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="randNum">The rand number.</param>
        /// <returns></returns>
        [HttpGet]
        public Uri GetPhoto(int? memberId = null, string userId = null, string randNum = null)
        {
            // Random number changes the image src url which forces
            // the browser to reload the image 

            string photoFileName = memberId != null ? unitOfWork.MemberRepository.FindMemberById(memberId).PhotoFileName
                : unitOfWork.AppUserRepository.FindUserById(userId).PhotoFileName;


            return _service.GetPhotoSrcUri(photoFileName);
            //return photo == null ? null : new FileContentResult(photo.Photo, photo.MimeType);
        }

        /// <summary>
        /// Deprecated since we're using blob storage.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="randNum">The rand number.</param>
        /// <returns></returns>
        [HttpGet]
        public string GetThumbnailImgSrc(int? memberId = null, string userId = null)
        {  
            string photoFileName = memberId != null ? unitOfWork.MemberRepository.FindMemberById(memberId).PhotoFileName
                  : unitOfWork.AppUserRepository.FindUserById(userId).PhotoFileName;

            return _service.GetPhotoSrcString(photoFileName);
        }

        [HttpGet]
        public string GetUserIconImgSrc(string userId)
        {
            string photoFileName = unitOfWork.AppUserRepository.FindUserById(userId).PhotoFileName;
            if (!string.IsNullOrEmpty(photoFileName) )
                return _service.GetPhotoSrcString(photoFileName);
            return "";
        }
        /// <summary>
        /// Updates the image src if image is updated.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public string UpdatePhotoImgSrc(int? memberId=null, string userId=null)
        {
            bool hasPhoto = false;          
            if (memberId != null)
            {
                var member = unitOfWork.MemberRepository.FindMemberById(memberId);               
                hasPhoto = _service.DoesMemberHavePhoto(memberId: memberId);
                if (hasPhoto) return _service.GetPhotoSrcString(member.PhotoFileName);
            }
            else
            {
                var user = unitOfWork.AppUserRepository.FindUserById(userId);              
                hasPhoto = _service.DoesMemberHavePhoto(userId: userId);
                if (hasPhoto) return _service.GetPhotoSrcString(user.PhotoFileName);
            }

            return string.Empty;

        }
        /// <summary>
        /// Deprecated since we're using blob storage.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateViewThumbnail(int? memberId=null, string userId=null)
        {
            return RedirectToAction("GetThumbnail", new { memberId = memberId, userId=userId });
        }

        // GET
        public ActionResult Edit(int? memberId=null, string userId=null)
        {
            if (memberId == null && userId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string fullName = "";           
            bool hasPhoto = false;
            string imageSrc = string.Empty;
            if (memberId != null)
            {
                var member = unitOfWork.MemberRepository.FindMemberById(memberId);
                fullName = $"{member.FirstName} {member.LastName}";
                hasPhoto = _service.DoesMemberHavePhoto(memberId: memberId);
                if (hasPhoto) imageSrc = _service.GetPhotoSrcString(member.PhotoFileName);
            }
            else
            {
                var user = unitOfWork.AppUserRepository.FindUserById(userId);
                fullName = $"{user.FirstName} {user.LastName}";
                hasPhoto = _service.DoesMemberHavePhoto(userId: userId);
                if (hasPhoto) imageSrc = _service.GetPhotoSrcString(user.PhotoFileName);
            }


            var viewModel = new MemberPhotoEditViewModel
            {
                MemberId = memberId,
                UserId = userId,
                FullName = fullName,
                HasPhoto = hasPhoto,
                ImageSrc = imageSrc
            };

            return View(viewModel);
        }

        //[HttpPost]
        public ActionResult DeletePhoto(int? memberId=null, string userId=null)
        {
            // Redirect to home page if id is null
            if (memberId == null && userId == null)
                return RedirectToAction("Index", "Home");

            // If photo exists, delete it
            if (memberId != null && _service.DoesMemberHavePhoto(memberId: memberId)) 
                _service.DeletePhoto(memberId: memberId);
            else if (userId != null && _service.DoesMemberHavePhoto(userId: userId))               
                _service.DeletePhoto(userId: userId); 

            return RedirectToAction("Edit", new { memberId = memberId, userId = userId });
        }
    }
}