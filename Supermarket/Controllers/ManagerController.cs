using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Web.Security;
using Supermarket.Models;
using System.Configuration;

namespace Supermarket.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();
        // GET: Manager
        public ActionResult Index()
        {
            var products = _dbContext.Products;
            return View(products.ToList());
        }

        public ActionResult AddProduct()
        {
            ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(NewProduct newProduct)
        {
            try
            {
                if (newProduct.picture.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(newProduct.picture.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/productImages"), _FileName);
                    newProduct.picture.SaveAs(_path);
                    newProduct.product.imageURL = _FileName;
                }
                else
                {
                    newProduct.product.imageURL = "null.png";
                }
                newProduct.product.productID = Guid.NewGuid();
                newProduct.product.category = newProduct.category;
                _dbContext.Products.Add(newProduct.product);
                _dbContext.SaveChanges();
                return RedirectToAction("Index", "Manager");
            }
            catch
            {
                ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName");
                ViewBag.FileMessage = "failed for some reason...";
                return View();
            }
        }
    }
}