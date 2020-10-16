using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supermarket.Models
{
    public class OrderProducts
    {
        public Order order { get; set; }

        public List<Order_Product> orderItems { get; set; }
    }
}