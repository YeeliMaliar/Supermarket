using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supermarket.Models
{
    public class NewCategory
    {
        [Required]
        public Category category { get; set; }

        public HttpPostedFileBase NewPicture { get; set; }

        public ProductImage oldPicture { get; set; }
    }
}