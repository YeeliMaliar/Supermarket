using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Supermarket.Models
{
    public class ShoppingCart
    {
        private readonly SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";
        
        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
        
        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Product product, int qty)
        {
            // Get the matching cart and album instances
            var cartItem = _dbContext.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ProductId == product.productID);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    RecordId = Guid.NewGuid(),
                    ProductId = product.productID,
                    CartId = ShoppingCartId,
                    count = qty,
                    DateCreated = DateTime.Now
                };
                _dbContext.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.count += qty;
            }
            // Save changes
            _dbContext.SaveChanges();
        }

        public int RemoveFromCart(Guid id, int qty)
        {
            // Get the cart
            var cartItem = _dbContext.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null || cartItem.count == qty)
            {
                if (qty != 0)
                {
                    cartItem.count = qty;
                    itemCount = cartItem.count;
                    _dbContext.Entry(cartItem).State = EntityState.Modified;
                }
                else
                {
                    _dbContext.Carts.Remove(cartItem);
                }
                // Save changes
                _dbContext.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = _dbContext.Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                _dbContext.Carts.Remove(cartItem);
            }
            // Save changes
            _dbContext.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return _dbContext.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _dbContext.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            // Multiply the price by count of that product to get 
            // the current price for each of those products in the cart
            // sum all product price totals to get the cart total
            decimal? total = (from cartItems in _dbContext.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.count *
                              cartItems.Product.price).Sum();

            return total ?? decimal.Zero;
        }

        public Order CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new Order_Product
                {
                    productID = item.ProductId,
                    orderId = order.orderId,
                    unitPrice = item.Product.price,
                    quantity = item.count,
                    orderProductID = Guid.NewGuid()
                    
                };
                // Set the order total of the shopping cart
                orderTotal += (item.count * item.Product.price);
                _dbContext.Order_Product.Add(orderDetail);

            }
            // Set the order's total to the orderTotal count
            order.checkoutTotal = orderTotal;
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order;
        }

        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            // ???
            return context.Session[CartSessionKey].ToString();
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart =_dbContext.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            _dbContext.SaveChanges();
        }
    }
}