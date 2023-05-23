using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class EquipmentTypeModel
    {
        public int Id { get; set; }


        public string Code { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Level")]
        public string Level { get; set; }


    }
}