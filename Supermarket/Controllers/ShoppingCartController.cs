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
        SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();

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

        public ActionResult AddToCart(Guid id)
        {
            // Retrieve the product from the database
            var addedProduct = _dbContext.Products
                .Single(product => product.productID == id);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(addedProduct);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index", "ShoppingCart");
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