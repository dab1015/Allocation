using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class FunctionModel
    {
        public int Id { get; set; }


        public string Code { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Function Name")]
        public string FunctionName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }


    }
}