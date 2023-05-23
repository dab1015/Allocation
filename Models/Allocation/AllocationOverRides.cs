using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class AllocationOverRides
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Department")]
        public int Department { get; set; }

        public int SubDepartment { get; set; }

        

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Code")]
        public int Code { get; set; }

        public int AllSearch { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Department")]
        public string DeptName { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }


        public int SubCode { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Sub-Department")]
        public string SubDeptName { get; set; }

        public int ClassCode { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Class")]
        public string ClassName { get; set; }

        public int SubClassCode { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Sub-Class")]
        public string SubClassName { get; set; }


        public string VendorName { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Vendor Code")]
        public int VendorCode { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "SKU")]
        public long SKU { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string ClubNames { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Multiplier Set-Up")]
        public int MultiplierSetup { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Current Multiplier")]
        public int CurrentMultiplier { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Multiplier Adjustment")]
        public int MultiplierAdjustment { get; set; }


        public string Club { get; set; }

        public int ClubCode { get; set; }

        public List<AllocationOverRides> OverRides { get; set; }

       

       


    }
}