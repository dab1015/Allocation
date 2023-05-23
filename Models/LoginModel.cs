using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SNRWMSPortal.Models
{
    public class LoginModels
    {

        [Required(ErrorMessage = "❌ Username field is required")]
        [Display(Name = "UsernameLogin")]
        public string Username { get; set; }

        [RegularExpression(@"^[^,:*?""<>\|]*$")]
        //[RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\da-zA-Z])(.{8,15})$",ErrorMessage ="Must have 1 Capital Letter, Special Character")]
        [Required(ErrorMessage = "❌ Password field is required")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "❌ The password must at least be 6 characters")]
        
        [Display(Name = "PasswordLogin")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}