using MvcFileUploader.Models;
using System;

namespace DeaconCCGManagement.ViewModels
{
    public class MemberPhotoEditViewModel : FileUploadViewModel
    {
        public int Id { get; set; }
        public int? MemberId { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public bool HasPhoto { get; set; }
        public string ImageSrc { get; set; }
    }
}