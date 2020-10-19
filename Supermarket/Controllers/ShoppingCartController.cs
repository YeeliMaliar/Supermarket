using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Supermarket.Models;
using Supermarket.ViewModels;

namespace Supermarket.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddToCart(Guid id, int qty)
        {
            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Retrieve the product from the database
            var addedProduct = _dbContext.Products
                .Single(product => product.productID == id);


            cart.AddToCart(addedProduct, qty);

            string results = "Item added to cart";

            // Display the confirmation message
            return Json(results);
        }

        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult UpdateCart(Guid id, int qty)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the product to display confirmation
            string productName = _dbContext.Carts
                .Single(item => item.RecordId == id).Product.name;
            

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id, qty);

            // Display the confirmation message
            var results = new ShoppingCartUpdateViewModel
            {
                Message = Server.HtmlEncode(productName) +
                    " updated",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}