using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Supermarket.Models
{
    public class PaymentModel
    {
        [Required]
        public string Code { get; set; }
    }
}