using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class RegisterEquipment
    {

        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Equipment ID")]
        public string EquipmentDesc { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Equipment")]
        public string EquipmentType { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Primary Function")]
        public int PrimaryFunc { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Secondary Function")]
        public int SecondaryFunc { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Tertiary Function")]
        public int TertiaryFunc { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        public List<RegisterEquipment> Equipments { get; set; }

    }
}