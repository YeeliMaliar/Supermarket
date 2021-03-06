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

    public partial class Address
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Address()
        {
            this.Users = new HashSet<User>();
        }

        public System.Guid addressID { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(50, ErrorMessage = "The city name can't contain more than 50 letters")]
        public string city { get; set; }

        [Display(Name = "Street")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(50, ErrorMessage = "The street name can't contain more than 50 letters")]
        public string street { get; set; }

        [Display(Name = "House number")]
        [Required(ErrorMessage = "This Field is required")]
        [Range(0, 500, ErrorMessage = "The house number can't be negative or higher than 500")]
        public int houseNumber { get; set; }

        [Display(Name = "floor")]
        [Range(0, 100, ErrorMessage = "The floor number can't be negative or higher than 100")]
        public Nullable<int> floor { get; set; }

        [Display(Name = "apartment")]
        [Range(0, 400, ErrorMessage = "The apartment number can't be negative or higher than 400")]
        public Nullable<int> apartment { get; set; }

        [Display(Name = "Postal code")]
        [Required(ErrorMessage = "This Field is required")]
        [RegularExpression(@"\d{7}", ErrorMessage = "Postal code must contain numbers only and can't be longer than 7 digits")]
        public string postalCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}
