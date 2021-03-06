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

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Carts = new HashSet<Cart>();
            this.Order_Product = new HashSet<Order_Product>();
        }

        [Display(Name = "Product ID")]
        public System.Guid productID { get; set; }
        [Display(Name = "Product name")]
        [Required(ErrorMessage = "This Field is required")]
        [StringLength(100)]
        public string name { get; set; }
        [Display(Name = "Price")]
        [Required(ErrorMessage = "This Field is required")]
        public decimal price { get; set; }
        [Display(Name = "Product description")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }
        [Display(Name = "Quantity in stock")]
        [Required(ErrorMessage = "This Field is required")]
        public int stock { get; set; }
        [Display(Name = "Categoty number")]
        public int category { get; set; }
        public Nullable<System.Guid> ImageID { get; set; }
        public Nullable<int> available { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual Category Category1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Product> Order_Product { get; set; }
        public virtual ProductImage ProductImage { get; set; }
    }
}
