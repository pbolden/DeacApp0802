using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeaconCCGManagement.Test_Data
{
    public class Member
    {
        public string IndividualId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string Suffix { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }
        public string PreferredEmail { get; set; }
        public string FamilyNumber { get; set; }
        public string EnvelopeNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateJoined { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? DateLastChanged { get; set; }
        public string FamDistrictDeacon { get; set; }
        public string CCG { get; set; }
    }
}
