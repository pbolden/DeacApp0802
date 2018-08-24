using System;

namespace DeaconCCGManagement.Models
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? EventDate { get; set; }
        public string DateString { get; set; }
        public string Url { get; set; }
        public string PhoneNumber { get; set; }
        public string CellPhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}