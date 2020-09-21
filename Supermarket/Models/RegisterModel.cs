using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supermarket.Models
{
    public class RegisterModel
    {
        [Required]
        public User User { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100)]
        [Display(Name = "Password")]
        [Compare("password_conf", ErrorMessage = "password must match")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", 
            ErrorMessage ="Password must contain at least one capital letter, one lower letter, one digit, one sybol and be at least 8 charcters long.")]
        public string password { get; set; }


        [Required(ErrorMessage ="This field is required")]
        [StringLength(100)]
        [Display(Name = "Confirme password")]
        public string password_conf { get; set; }
    }
}