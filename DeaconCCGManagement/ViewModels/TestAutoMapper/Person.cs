using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeaconCCGManagement.ViewModels.TestAutoMapper
{
    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Pet> PetNames { get; set; }
    }
}