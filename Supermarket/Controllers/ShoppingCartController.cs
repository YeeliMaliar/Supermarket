using System;
using System.Collections.Generic;
using System.Data.Entity;
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

            ViewBag.StockMessage = TempData["StockMessage"];
            TempData.Remove("StockMessage");

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }

        [Authorize]
        public ActionResult CheckOut()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);
            List<Cart> CartItems = cart.GetCartItems();
            if (CartItems.Count == 0)
            {
                TempData["StockMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }
            foreach (var item in CartItems)
            {
                if (item.count > item.Product.stock)
                {
                    TempData["StockMessage"] = "One or more item exceed our quantity in stock. please update your cart and try again";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(PaymentModel PaymentDetails)
        {
            if (ModelState.IsValid)
            {
                if (PaymentDetails.Code == "Pay") // "payment detail" validation
                {
                    // "money transfer" goes here

                    // update stock, clear cart, create order
                    var cart = ShoppingCart.GetCart(this.HttpContext);
                    List<Cart> CartItems = cart.GetCartItems();
                    foreach (var item in CartItems)
                    {
                        item.Product.stock -= item.count;
                    }

                    // get user:
                    var user = _dbContext.Users.Where(u => u.emailAddress == User.Identity.Name).FirstOrDefault();

                    //new order:
                    Order newOrder = new Order
                    {
                        checkoutTime = DateTime.UtcNow,
                        userID = user.userID
                    };
                    _dbContext.Orders.Add(newOrder);
                    _dbContext.SaveChanges();
                    newOrder = cart.CreateOrder(newOrder);
                    _dbContext.Entry(newOrder).State = EntityState.Modified;
                    // Save the order
                    _dbContext.SaveChanges();

                    return RedirectToAction("OrderComplete", new { id = newOrder.orderId });
                }

                else
                {
                    ViewBag.PaymentMessage = "Payment details invalid, purchase failed. Please try again.";
                    return View();
                }
            }
            else
            {
                return View();
            }
            
        }

        // AJAX: /ShoppingCart/AddtoCart
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

        // AJAX: /ShoppingCart/UpdateCart
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

        [Authorize]
        public ActionResult OrderComplete(long id)
        {
            bool isValid = _dbContext.Orders.Any( o => o.orderId == id && o.User.emailAddress == User.Identity.Name);

            if (isValid)
            {
                ViewBag.idNumber = id;
                return View();
            }
            else
            {
                return RedirectToAction("AccessDenied", "ErrorHandler");
            }
        }


    }
}