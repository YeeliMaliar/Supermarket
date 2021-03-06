﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Supermarket.ViewModels
{
    public class ShoppingCartUpdateViewModel
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public Guid DeleteId { get; set; }
    }
}