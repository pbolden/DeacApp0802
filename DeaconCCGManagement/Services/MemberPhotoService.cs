using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using DeaconCCGManagement.BlobStorage;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Models;
using MvcFileUploader.Models;
using DeaconCCGManagement.ViewModels;
using DeaconCCGManagement.Helpers;

namespace DeaconCCGManagement.Services
{
    public class MemberPhotoService
    {
        private readonly UnitOfWork unitOfWork;
        private static Random randomizer;
        private BlobStorageReposity blobRepo;

        public MemberPhotoService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            blobRepo = new BlobStorageReposity();
            randomizer = new Random();
        }

        public Uri GetPhotoSrcUri(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;
            return blobRepo.GetBlobUri(fileName);   
        }

        public string GetPhotoSrcString(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            var uri = GetPhotoSrcUri(fileName);
            if (uri == null) return ViewHelper.NoPhotoImgPath;
            return uri.ToString();
        }

        public bool DoesMemberHavePhoto(int? memberId = null, string userId = null)
        {          
            string photoFileName = memberId != null ? unitOfWork.MemberRepository.FindById(memberId).PhotoFileName
                : unitOfWork.AppUserRepository.FindUserById(userId).PhotoFileName;

            return !string.IsNullOrEmpty(photoFileName);
        }
        /// <summary>
        /// Deletes photo from blob storage and deletes name from database.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public bool DeletePhoto(int? memberId = null, string userId = null)
        {
            if (memberId != null)
            {
                var member = unitOfWork.MemberRepository.FindById(memberId);
                string photoFileName = member.PhotoFileName;
                member.PhotoFileName = null;
                unitOfWork.MemberRepository.Update(member);
                return blobRepo.DeleteBlob(photoFileName);
            }
            else if (userId != null)
            {
                var user = unitOfWork.AppUserRepository.FindUserById(userId);
                user.PhotoFileName = null;
                unitOfWork.AppUserRepository.Update(user);
                return blobRepo.DeleteBlob(user.PhotoFileName);
            }

            return false;
        }
        /// <summary>
        /// Stores the image to blob and saves name to database.
        /// </summary>
        /// <param name="httpPostedFile">The HTTP posted file.</param>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public void StoreImage(HttpPostedFileBase httpPostedFile, int? memberId=null, string userId=null)
        {
            if (httpPostedFile == null || httpPostedFile.ContentLength == 0) return;

            string newFileName = BlobService.GetNewFileName(httpPostedFile.FileName);         

            // Save image to blob and save name to database.
            if (memberId != null)
            {
                // If member already has a photo, remove it from blob
                var member = unitOfWork.MemberRepository.FindMemberById(memberId);
                if (!string.IsNullOrEmpty(member.PhotoFileName))              
                    blobRepo.DeleteBlob(member.PhotoFileName);                

                member.PhotoFileName = newFileName;
                unitOfWork.MemberRepository.Update(member);
                blobRepo.UploadBlob(httpPostedFile, newFileName);
            }
            else if (!string.IsNullOrEmpty(userId)) // App user photo
            {
                // If app user already has a photo, remove it from blob
                var user = unitOfWork.AppUserRepository.FindUserById(userId);
                if (!string.IsNullOrEmpty(user.PhotoFileName))
                    blobRepo.DeleteBlob(user.PhotoFileName);

                user.PhotoFileName = newFileName;
                unitOfWork.AppUserRepository.Update(user);
                blobRepo.UploadBlob(httpPostedFile, newFileName);
            }          
        }

        private void UpdateExistingPhoto(MemberPhoto existingPhoto, MemberPhoto photo)
        {
            // delete existing photo in container
            // store new file name in table
            // upload new photo to blob



            //existingPhoto.Photo = photo.Photo;
            //existingPhoto.Thumbnail = photo.Thumbnail;
            //existingPhoto.MimeType = photo.MimeType;
            //db.ImageRepository.Update(existingPhoto);
        }

        public MemberPhoto GetMemberPhoto(HttpPostedFileBase httpPostedFile, int? memberId=null, string userId=null)
        {
            // Get file's mime type
            var mimeType = httpPostedFile.ContentType;

            // Get file's input stream
            var stream = httpPostedFile.InputStream;

            // Resize image, set wallet and thumbnail images
            var img = new WebImage(stream);            

            // Get bytes from WebImage object
            var photoBytes = GetImageBytes(img, 160, 160);
            var thumbnailBytes = GetImageBytes(img, 80, 80);

            return new MemberPhoto
            {
                Photo = photoBytes,
                Thumbnail = thumbnailBytes,
                MemberId = memberId,
                AppUserId = userId,
                MimeType = mimeType
            };
        }

        public byte[] GetImageBytes(WebImage img, int width, int height)
        {            
            if (img.Width > width)
                img.Resize(width, height, false);
            return img.GetBytes();
        }

        public MemberPhoto CreateMemberPhotoObject(MemberPhoto photo)
        {
            var image = new MemberPhoto
            {
                MemberId = photo.MemberId,
                AppUserId = photo.AppUserId,
                Photo = photo.Photo,
                Thumbnail = photo.Thumbnail,
                MimeType = photo.MimeType
            };
            return image;
        }

        //public void DeleteMemberPhotos(int? memberId=null, string userId=null)
        //{
        //    // Delete any member photos that may already exist.
            
        //    //var currentPhotos = memberId != null ? db.ImageRepository.FindAll(img => img.MemberId == memberId).ToList() 
        //    //    : db.ImageRepository.FindAll(img => img.AppUserId.Equals(userId)).ToList();

        //    //foreach (var currentPhoto in currentPhotos)
        //    //{
        //    //    db.ImageRepository.Delete(currentPhoto);
        //    //}
        //}

        public string GetRandomNumber()
        {
            int randNum = randomizer.Next(1000000, 9000000);
            return randNum.ToString();
        }

        public ViewDataUploadFileResult GetUploadFileResult()
        {
            return new ViewDataUploadFileResult();
        }
    }
}