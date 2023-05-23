using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class ProfileModel
    {
        public int Id { get; set; }


        public string Code { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Profile")]
        public string Profile { get; set; }
    }
}