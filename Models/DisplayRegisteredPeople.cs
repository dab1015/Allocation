using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class DisplayRegisteredPeople
    {
        public int Id { get; set; }
        public string UserID { get; set; }


        public string FirstName { get; set; }


        public string LastName { get; set; }



        public string EmployeeNo { get; set; }


        public string Designation { get; set; }


        public string Profile { get; set; }

        public bool isActiveDirectory { get; set; }
        public string Status { get; set; }

        public List<DisplayRegisteredPeople> Peoples { get; set; }

        public IEnumerable<SelectListItem> ItemsSelected { get; set; }

        [DataType(DataType.Password)]
        public string Hash { get; set; }

        public List<SelectListItem> ProfileItems { get; set; }
    }
}