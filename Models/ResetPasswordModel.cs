using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SNRWMSPortal.Models
{
    public class ResetPasswordModel
    {

        [Required(ErrorMessage = "❌ Current password field is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]

        public string currentPassword { get; set; }


        [Required(ErrorMessage = "❌ New password field is required")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\da-zA-Z])(.{8,15})$", ErrorMessage = "❌ Must have 1 Capital Letter, 1 Special Character")]
        //[Required(ErrorMessage = "This field is required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "❌ The password must at least be 8 characters")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]

        public string newPassword { get; set; }

        [Required(ErrorMessage = "❌ Confirm password field is required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "❌ The password must at least be 8 characters")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("newPassword", ErrorMessage = "❌ Confirm password doesnt match, try again.")]
        public string confirmNewPassword { get; set; }

        public int resetStatus { get; set; }
    }
}