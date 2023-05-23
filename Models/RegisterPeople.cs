using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class RegisterPeople
    {

        public int Id { get; set; }

        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "User ID")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "❌ New password field is required")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\da-zA-Z])(.{8,15})$", ErrorMessage = "❌ Must have 1 Capital Letter, 1 Special Character")]
        //[Required(ErrorMessage = "This field is required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "❌ The password must at least be 8 characters")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Hash { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Employee Number")]
        public string EmployeeNo { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Designation")]
        public string Designation { get; set; }


        [Display(Name = "Active Directory")]
        public bool isActiveDirectory { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Profile")]
        public List<string> Profile { get; set; }
        public IEnumerable<SelectListItem> ItemsSelected { get; set; }

        //public string Profile { get; set; }

        //   [Required(ErrorMessage = "This field is required")]
        //  [Display(Name = "Equipment")]
        //  public int Equipment { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Status")]
        public int Status { get; set; }

        public List<DisplayRegisteredPeople> Peoples { get; set; }

    }
}