using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Web.Security;
using Supermarket.Models;
using System.Configuration;
using System.Net;
using System.Web.Mvc.Ajax;

namespace Supermarket.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();
        
        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Products()
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
            // valid file Extensions:
            var allowedExtensions = new[] {
            ".Jpg", ".png", ".jpg", "jpeg"};

            // get the file's full name and extension
            string _FileName = Path.GetFileName(newProduct.picture.FileName);
            var _Ext = Path.GetExtension(newProduct.picture.FileName);

            // check if a picture is provided
            if (newProduct.picture.ContentLength > 0)
            {
                // check if the file provided is valid
                if (allowedExtensions.Contains(_Ext))
                {
                    //getting file name without extension  
                    string name = Path.GetFileNameWithoutExtension(_FileName);

                    // create a new image item and it's ID
                    ProductImage newImage = new ProductImage();
                    newImage.ImageID = Guid.NewGuid();
                    

                    // create a new filename ( name_ImageID.ext)
                    string myfile = name + "_" + newImage.ImageID + _Ext;

                    //create a complete path for saving
                    string _path = Path.Combine(Server.MapPath("~/Content/productImages"), myfile);

                    //set the image obj name and date
                    newImage.imageName = myfile;
                    newImage.UploadDate = DateTime.UtcNow;

                    // gives the new product an ID
                    newProduct.product.productID = Guid.NewGuid();

                    //set the images product to the new product, then adds it to the db
                    _dbContext.ProductImages.Add(newImage);

                    //save the picture in the server
                    newProduct.picture.SaveAs(_path);

                    newProduct.product.ImageID = newImage.ImageID;

                    //set the new products category, than add the product to the db
                    newProduct.product.category = newProduct.category;
                    _dbContext.Products.Add(newProduct.product);

                    // save and return to the list
                    _dbContext.SaveChanges();
                    return RedirectToAction("Products", "Manager");
                }
                //the user chose and invalid file, return to the view with a message
                else
                {
                    ViewBag.message = "Please choose only an Image file";
                    ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName");
                    return View();
                }
            }
            // no image is uploaded, handle it later to show "null.png"
            else
            {
                
                // gives the new product an ID
                newProduct.product.productID = Guid.NewGuid();

                //set the new products category, than add the product to the db
                newProduct.product.category = newProduct.category;
                _dbContext.Products.Add(newProduct.product);

                // save and return to the list
                _dbContext.SaveChanges();
                return RedirectToAction("Products", "Manager");
            }
        }

    }
}