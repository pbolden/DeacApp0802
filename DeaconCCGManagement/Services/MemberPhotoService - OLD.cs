//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Helpers;
//using DeaconCCGManagement.DAL;
//using DeaconCCGManagement.Models;
//using MvcFileUploader.Models;

//namespace DeaconCCGManagement.Services
//{
//    public class MemberPhotoService
//    {
//        private readonly UnitOfWork db;
//        private static Random randomizer;

//        public MemberPhotoService(UnitOfWork db)
//        {
//            this.db = db;
//            randomizer = new Random();
//        }

//        public bool DoesMemberHavePhoto(int? memberId=null, string userId=null)
//        {
//            return memberId != null ? db.ImageRepository.Exists(p => p.MemberId == memberId)
//                : db.ImageRepository.Exists(p => p.AppUserId.Equals(userId));
//        }

//        public void StoreImage(HttpPostedFileBase httpPostedFile, int? memberId=null, string userId=null)
//        {
//            // Get data from posted file; return MemberPhoto obj.
//            var photo = GetMemberPhoto(httpPostedFile, memberId, userId);

//            // Save image to database.

//            if (photo.Photo == null || photo.Thumbnail == null) return;

//            // If member already has a photo, update it
//            if (memberId != null && db.ImageRepository.Exists(img => img.MemberId == photo.MemberId))
//            {
//                var existingPhoto = db.ImageRepository.FindByMemberId(photo.MemberId);
//                UpdateExistingPhoto(existingPhoto, photo);
//            }
//            else if (!string.IsNullOrEmpty(userId) 
//                && db.ImageRepository.Exists(img => img.AppUserId.Equals(photo.AppUserId))) // App user photo
//            {
//                var existingPhoto = db.ImageRepository.FindByUserId(photo.AppUserId);
//                UpdateExistingPhoto(existingPhoto, photo);
//            }
//            else
//            {
//                //var newPhoto = CreateMemberPhotoObject(photo);
//                db.ImageRepository.Add(photo);
//            }
//        }

//        private void UpdateExistingPhoto(MemberPhoto existingPhoto, MemberPhoto photo)
//        {
//            existingPhoto.Photo = photo.Photo;
//            existingPhoto.Thumbnail = photo.Thumbnail;
//            existingPhoto.MimeType = photo.MimeType;
//            db.ImageRepository.Update(existingPhoto);
//        }

//        private MemberPhoto GetMemberPhoto(HttpPostedFileBase httpPostedFile, int? memberId=null, string userId=null)
//        {
//            // Get file's mime type
//            var mimeType = httpPostedFile.ContentType;

//            // Get file's input stream
//            var stream = httpPostedFile.InputStream;

//            // Resize image, set wallet and thumbnail images
//            var img = new WebImage(stream);

//            // Get bytes from WebImage object
//            var photoBytes = GetImageBytes(img, 160, 160);
//            var thumbnailBytes = GetImageBytes(img, 80, 80);

//            return new MemberPhoto
//            {
//                Photo = photoBytes,
//                Thumbnail = thumbnailBytes,
//                MemberId = memberId,
//                AppUserId = userId,
//                MimeType = mimeType
//            };
//        }

//        private byte[] GetImageBytes(WebImage img, int width, int height)
//        {
//            if (img.Width > width)
//                img.Resize(width, height, false);
//            return img.GetBytes();
//        }

//        public MemberPhoto CreateMemberPhotoObject(MemberPhoto photo)
//        {
//            var image = new MemberPhoto
//            {
//                MemberId = photo.MemberId,
//                AppUserId = photo.AppUserId,
//                Photo = photo.Photo,
//                Thumbnail = photo.Thumbnail,
//                MimeType = photo.MimeType
//            };
//            return image;
//        }

//        public void DeleteMemberPhotos(int? memberId=null, string userId=null)
//        {
//            // Delete any member photos that may already exist.
            
//            var currentPhotos = memberId != null ? db.ImageRepository.FindAll(img => img.MemberId == memberId).ToList() 
//                : db.ImageRepository.FindAll(img => img.AppUserId.Equals(userId)).ToList();

//            foreach (var currentPhoto in currentPhotos)
//            {
//                db.ImageRepository.Delete(currentPhoto);
//            }
//        }

//        public string GetRandomNumber()
//        {
//            int randNum = randomizer.Next(1000000, 9000000);
//            return randNum.ToString();
//        }

//        public ViewDataUploadFileResult GetUploadFileResult()
//        {
//            return new ViewDataUploadFileResult();
//        }
//    }
//}