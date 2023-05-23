using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNRWMSPortal.Models
{
    public class EmployeeDetails
    {

        public int EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
    }
}