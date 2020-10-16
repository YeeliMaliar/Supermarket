using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supermarket.Models
{
    public class LoginModel
    {
       
        [Display(Name = "Email address")]
        [Required(ErrorMessage = "This Field is required")]
        [EmailAddress(ErrorMessage = "Email address must be valid")]
        [StringLength(50)]
        public string email { get; set; }


        [Display(Name = "Password")]
        [Required(ErrorMessage = "This field is required")]
        [StringLength(100)]
        public string password { get; set; }

    }
}