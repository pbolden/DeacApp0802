using DeaconCCGManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeaconCCGManagement.DeaconAnnouncements
{
    /// <summary>
    /// Helper methods for the deacon announcement feature.
    /// </summary>
    public static class AnnouncementHelper
    {
        static AnnoucementRepository _repository;

        public static bool CreatedNewAnnouncement { get; set; }

        public static bool UpdatedAnnouncement { get; set; }

        // same partition key for all announcements
        private static string _partitionKey = "deaconannouncements";

        public static List<AnnouncementViewModel> GetAllAnnouncements()
        {
            if (_repository == null) _repository = new AnnoucementRepository();

            RemoveOldAnnouncements();
            var announcementsViewModel = new List<AnnouncementViewModel>();
            var announcements = _repository.GetAllAnnouncements(_partitionKey).ToList();

            // map to view model
            foreach (var announcement in announcements)
            {
                announcementsViewModel.Add(new AnnouncementViewModel
                {
                    ExpirationDate = announcement.ExpirationDate,
                    TimeStamp = announcement.Timestamp.DateTime,
                    Title = announcement.Title,
                    AnnouncementHtml = announcement.AnnouncementHtml,
                    PartitionKey = announcement.PartitionKey,
                    RowKey = announcement.RowKey
                });
            }

            announcementsViewModel.Sort();
            return announcementsViewModel;
        }

        private static void RemoveOldAnnouncements()
        {
            if (_repository == null) _repository = new AnnoucementRepository();

            var announcements = _repository.GetAllAnnouncements(_partitionKey).ToList();

            foreach (var announcement in announcements)
            {
                if (announcement.ExpirationDate < DateTime.Now)                
                    _repository.RemoveAnnouncement(announcement.PartitionKey, announcement.RowKey);                
            }
        }

        public static void AddAnnouncent(AnnouncementViewModel newAnnouncement)
        {
            if (newAnnouncement == null) return;

            if (_repository == null) _repository = new AnnoucementRepository();
        

            // map view model to model
            var announcement = new AnnouncementModel(_partitionKey, Guid.NewGuid().ToString())
            {
                ExpirationDate = newAnnouncement.ExpirationDate,
                Title = newAnnouncement.Title,
                AnnouncementHtml = newAnnouncement.AnnouncementHtml
            };

            _repository.AddAnnouncement(announcement);
        }

        public static void UpdateAnnouncent(AnnouncementViewModel announcement)
        {
            if (announcement == null) return;

            if (_repository == null) _repository = new AnnoucementRepository();


            // map view model to model
            var announcementToUpdate = new AnnouncementModel(announcement.PartitionKey, announcement.RowKey)
            {
                ExpirationDate = announcement.ExpirationDate,
                Title = announcement.Title,
                AnnouncementHtml = announcement.AnnouncementHtml
            };

            _repository.AddAnnouncement(announcementToUpdate);
        }


        public static AnnouncementViewModel GetAnnouncementViewModel(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey)) return null;
            if (_repository == null) _repository = new AnnoucementRepository();

            var announcement = _repository.GetAnnouncementByKey(partitionKey, rowKey);
            return new AnnouncementViewModel
            {
                ExpirationDate = announcement.ExpirationDate,
                Title = announcement.Title,
                AnnouncementHtml = announcement.AnnouncementHtml,
                PartitionKey = announcement.PartitionKey,
                RowKey = announcement.RowKey
            };
        }

        public static void DeleteAnnouncement(string partitionKey, string rowKey)
        {
            if (_repository == null) _repository = new AnnoucementRepository();

            _repository.RemoveAnnouncement(partitionKey, rowKey);
        }

    }
}