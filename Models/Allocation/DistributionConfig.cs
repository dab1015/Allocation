
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class DistributionConfig
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Code")]
        public int Code { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Distribution Config")]
        public string Dconfig { get; set; }

        //[Required(ErrorMessage = "This field is required")]
        //[Display(Name = "Average Sales")]
        //public double AveSales { get; set; }

        public List<DistributionConfig> Configs { get; set; }
    }
}