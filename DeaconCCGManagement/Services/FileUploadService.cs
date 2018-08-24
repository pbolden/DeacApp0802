using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Extensions;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.Services
{
    public class FileUploadService
    {
        private readonly UnitOfWork db;
        public FileUploadService(UnitOfWork db)
        {
            this.db = db;
        }

        public byte[] GetFileData(HttpPostedData data, byte[] file, ref string mimeType)
        {
            if (!data.Files.ContainsKey(nameof(file))) return null;
            file = data.Files[nameof(file)].File;
            mimeType = data.Files[nameof(file)].MimeType;
            return file;
        }

        public bool GetFieldData(HttpPostedData data, ref int memberId)
        {
            if (!data.Fields.ContainsKey(nameof(memberId))) return false;
            string memberIdStr = data.Fields[nameof(memberId)].Value;
            return int.TryParse(memberIdStr, out memberId);
        }

        //public void PersistImage(byte[] file, int memberId, string mimeType)
        //{
        //    // Save image to database.
        //    if (file == null) return;
        //    var image = CreateMemberPhotoObject(file, memberId, mimeType);

        //    db.ImageRepository.Add(image);
        //}

        public MemberPhoto CreateMemberPhotoObject(byte[] file, int memberId, string mimeType)
        {
            var member = db.MemberRepository.FindMemberById(memberId);

            var image = new MemberPhoto
            {
                Member = member,
                Photo = file,
                Thumbnail = file,
                MimeType = mimeType
            };
            return image;
        }

        //public void DeleteMemberPhotos(int memberId)
        //{
        //    // Delete any member photos that may already exist.
        //    var currentPhotos = db.ImageRepository.FindAll(img => img.MemberId.Equals(memberId));
        //    foreach (var currentPhoto in currentPhotos)
        //    {
        //        db.ImageRepository.Delete(currentPhoto);
        //    }
        //}
    }
}