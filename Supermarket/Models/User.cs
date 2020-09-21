//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Supermarket.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Orders = new HashSet<Order>();
        }

        [Display(Name = "User ID")]
        public System.Guid userID { get; set; }


        [Display(Name = "Email")]
        [Required(ErrorMessage = "This Field is required")]
        [EmailAddress(ErrorMessage = "Email address must be valid")]
        [StringLength(50)]
        public string emailAddress { get; set; }


        [Display(Name = "First name")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(20)]
        public string firstName { get; set; }


        [Display(Name = "Last name")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(20)]
        public string lastName { get; set; }


        [Display(Name = "Phone number")]
        [Required(ErrorMessage = "This Field is required")]
        [Phone(ErrorMessage = "phone number must be valid")]
        public string phone { get; set; }



        public string passwordHash { get; set; }



        public string passwordSalt { get; set; }


        public int usertype { get; set; }


        [Display(Name = "City")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(50, ErrorMessage = "The city name can't contain more than 50 letters")]
        public string addressCity { get; set; }


        [Display(Name = "Street")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(50, ErrorMessage = "The street name can't contain more than 50 letters")]
        public string addressStreet { get; set; }


        [Display(Name = "House number")]
        [Required(ErrorMessage = "This Field is required")]
        [Range(0, 500, ErrorMessage = "The house number can't be negative or higher than 500")]
        public int addressHouse { get; set; }


        [Display(Name = "apartment")]
        [Range(0, 400, ErrorMessage = "The apartment number can't be negative or higher than 400")]
        public Nullable<int> addressApartment { get; set; }


        [Display(Name = "floor")]
        [Range(0, 100, ErrorMessage = "The floor number can't be negative or higher than 100")]
        public Nullable<int> addressFloor { get; set; }


        [Display(Name = "Postal code")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(7, ErrorMessage = "Postal code length can't be more than 7 numbers")]
        [RegularExpression(@"\d{7}", ErrorMessage = "Postal code must contain numbers only and can't be longer than 7 digits")]
        public string addressZip { get; set; }




        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
