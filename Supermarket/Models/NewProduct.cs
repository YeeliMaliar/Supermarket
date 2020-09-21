using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Supermarket.Models;
using System.ComponentModel;


namespace Supermarket.Models
{
    public class NewProduct
    {
        [Required]
        public Product product { get; set; }

        [Required]
        public int category { get; set; }

        public HttpPostedFileBase picture { get; set; }
    }
}