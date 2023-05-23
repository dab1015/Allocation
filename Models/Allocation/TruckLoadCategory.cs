
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class TruckLoadCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Code")]
        public int Code { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Truck Load Category")]
        public string CategoryName { get; set; }

        //[Required(ErrorMessage = "This field is required")]
        //[Display(Name = "Average Sales")]
        //public double AveSales { get; set; }

        public List<TruckLoadCategory> TruckCategories { get; set; }
    }
}