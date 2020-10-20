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
using Supermarket.ViewModels;
using System.Data.Entity;
using System.Text;
using System.Security.Cryptography;
using PagedList;

namespace Supermarket.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ManagerController : Controller
    {
        readonly SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();

        private readonly string globalSalt = "N2Pzd1tFN1";

        readonly string[] allowedExtensions = new[] { ".Jpg", ".png", ".jpg", "jpeg" };

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Products(string sortOrder, string currentFilter, string searchString, int? category, int? page)
        {
            // Viewbag:
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_Desc" : ""; //switch back and forth between those two values.
            ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_Desc" : "Price"; //switch back and forth between those two values.
            ViewBag.StockSortParm = sortOrder == "Stock" ? "Stock_Desc" : "Stock"; //switch back and forth between those two values.
            ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName");

            // handle search
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilterO = searchString;
            var products = _dbContext.Products.Include(c => c.Category1);
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(prd => prd.name.Contains(searchString)
                                       || prd.description.Contains(searchString));
            }

            // handle category
            ViewBag.CurrentCategory = null;
            if (category != null)
            {
                ViewBag.CurrentCategory = category;
                products = products.Where(prod => prod.category == category);
            }

            // sort
            switch (sortOrder)
            {
                case "Name_Desc":
                    products = products.OrderByDescending(prd => prd.name);
                    break;
                case "Price":
                    products = products.OrderBy(prd => prd.price);
                    break;
                case "Price_Desc":
                    products = products.OrderByDescending(prd => prd.price);
                    break;
                case "Stock":
                    products = products.OrderBy(prd => prd.stock);
                    break;
                case "Stock_Desc":
                    products = products.OrderByDescending(prd => prd.stock);
                    break;
                default:
                    products = products.OrderBy(prd => prd.name);
                    break;
            }

            // pagination
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Categories(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilterO = searchString;
            var categories = _dbContext.Categories.Include(c => c.ProductImage);
            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(cat => cat.categoryName.Contains(searchString));
            }
            categories = categories.OrderBy(cat => cat.categoryName);
            // pagination
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(categories.ToPagedList(pageNumber, pageSize));
        }

        #region Product management

        public ActionResult AddProduct()
        {
            ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(NewProduct newProduct)
        {
            if (ModelState.IsValid)
            {
                // check if a picture is provided
                if (newProduct.picture != null)
                {
                    // get the file's full name and extension
                    string _FileName = Path.GetFileName(newProduct.picture.FileName);
                    var _Ext = Path.GetExtension(newProduct.picture.FileName);

                    // check if the file provided is valid
                    if (allowedExtensions.Contains(_Ext))
                    {
                        //getting file name without extension  
                        string name = Path.GetFileNameWithoutExtension(_FileName);

                        // create a new image item and it's ID
                        ProductImage newImage = new ProductImage
                        {
                            ImageID = Guid.NewGuid()
                        };


                        // create a new filename ( name_ImageID.ext)
                        string myfile = name + "_" + newImage.ImageID + _Ext;

                        //create a complete path for saving
                        string _path = Path.Combine(Server.MapPath("~/Content/productImages"), myfile);

                        //set the image obj name and date
                        newImage.imageName = myfile;
                        newImage.UploadDate = DateTime.UtcNow;
                        newImage.imageType = _Ext;

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
                // no image is uploaded
                else
                {
                    newProduct.product.ImageID = Guid.Parse("D9BC92BA-5D32-4E39-89A9-BECEA428E1C9");
                    // gives the new product an ID
                    newProduct.product.productID = Guid.NewGuid(); //  link to image

                    //set the new products category, than add the product to the db
                    newProduct.product.category = newProduct.category;
                    _dbContext.Products.Add(newProduct.product);

                    // save and return to the list
                    _dbContext.SaveChanges();
                    return RedirectToAction("Products", "Manager");
                }
            }
            else
            {
                ViewBag.message = "model.state not valid";
                ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName");
                return View();
            }
        }

        public ActionResult EditProduct(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _dbContext.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName", product.category);

            NewProduct a = new NewProduct
            {
                product = product,
                oldPicture = _dbContext.ProductImages.Where(e => e.ImageID == product.ImageID).FirstOrDefault()
            };
            return View(a);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(NewProduct newProduct)
        {
            if (ModelState.IsValid)
            {
                newProduct.product.category = newProduct.category;
                
                // check if a picture is provided
                if (newProduct.picture != null)
                {
                    string _FileName = Path.GetFileName(newProduct.picture.FileName);
                    var _Ext = Path.GetExtension(newProduct.picture.FileName);

                    // check if the file provided is valid
                    if (allowedExtensions.Contains(_Ext))
                    {
                        //getting file name without extension  
                        string name = Path.GetFileNameWithoutExtension(_FileName);
                        string myfile, _path;

                        //update old image
                        if ((newProduct.oldPicture != null) && (newProduct.oldPicture.imageName != "noimage.jpg"))
                        {
                            // create a new filename ( name_ImageID.ext)
                            myfile = name + "_" + newProduct.oldPicture.ImageID + _Ext;

                            //create a complete path for saving
                            _path = Path.Combine(Server.MapPath("~/Content/productImages"), myfile);

                            //delete old picture
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/productImages"), newProduct.oldPicture.imageName));

                            //set the image obj name and date
                            newProduct.oldPicture.imageName = myfile;
                            newProduct.oldPicture.UploadDate = DateTime.UtcNow;
                            newProduct.oldPicture.imageType = _Ext;
                            _dbContext.Entry(newProduct.oldPicture).State = EntityState.Modified;

                        }
                        // create new image
                        else
                        {
                            ProductImage newimage = new ProductImage
                            {
                                ImageID = Guid.NewGuid(),
                                imageType = _Ext,
                                UploadDate = DateTime.UtcNow
                            };
                            newimage.imageName = name + "_" + newimage.ImageID + _Ext;
                            _path = Path.Combine(Server.MapPath("~/Content/productImages"), newimage.imageName);
                            _dbContext.ProductImages.Add(newimage);
                            newProduct.product.ImageID = newimage.ImageID;
                        }

                        //save the picture in the server
                        newProduct.picture.SaveAs(_path);
                    }
                    else
                    {
                        ViewBag.message = "Please choose only an Image file";
                        ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName", newProduct.product.category);
                        return View(newProduct);
                    }
                }
                
                _dbContext.Entry(newProduct.product).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Products");
            }
            ViewBag.message = "modelstate invalid";
            ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName", newProduct.product.category);
            return View(newProduct);
        }


        public ActionResult DeleteProduct(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _dbContext.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Product product = _dbContext.Products.Find(id);
            ProductImage image = _dbContext.ProductImages.Where(e => e.ImageID == product.ImageID).FirstOrDefault();
            if (image.imageName != "noimage.jpg")
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/productImages"), image.imageName));
                _dbContext.ProductImages.Remove(image);
            }
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return RedirectToAction("Products");
        }

        public ActionResult DeleteProductO(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _dbContext.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("DeleteProductO")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedO(Guid id)
        {
            Product product = _dbContext.Products.Find(id);
            ProductImage image = _dbContext.ProductImages.Where(e => e.ImageID == product.ImageID).FirstOrDefault();
            if (image.imageName != "noimage.jpg")
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/productImages"), image.imageName));
                _dbContext.ProductImages.Remove(image);
            }
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return RedirectToAction("OutofStock");
        }


        public ActionResult AddStock(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddStock a = new AddStock
            {
                product = _dbContext.Products.Find(id)
            };
            if (a.product == null)
            {
                return HttpNotFound();
            }
            
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStock(AddStock addStock)
        {
           
                Product product = _dbContext.Products.Where(e => e.productID == addStock.product.productID).FirstOrDefault();
                product.stock += addStock.added;
                _dbContext.Entry(product).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Products");
           
        }

        public ActionResult AddStockO(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddStock a = new AddStock
            {
                product = _dbContext.Products.Find(id)
            };
            if (a.product == null)
            {
                return HttpNotFound();
            }

            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStockO(AddStock addStock)
        {

            Product product = _dbContext.Products.Where(e => e.productID == addStock.product.productID).FirstOrDefault();
            product.stock += addStock.added;
            _dbContext.Entry(product).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return RedirectToAction("OutofStock");

        }

        public ActionResult OutofStock(string sortOrder, string currentFilter, string searchString, int? category, int? page)
        {
            // Viewbag:
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_Desc" : ""; //switch back and forth between those two values.
            ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_Desc" : "Price"; //switch back and forth between those two values.
            ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName");

            // handle search
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilterO = searchString;
            var products = _dbContext.Products.Where(model => model.stock <= 0);
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(prd => prd.name.Contains(searchString)
                                       || prd.description.Contains(searchString));
            }

            // handle category
            ViewBag.CurrentCategory = null;
            if (category != null)
            {
                ViewBag.CurrentCategory = category;
                products = products.Where(prod => prod.category == category);
            }

            // sort
            switch (sortOrder)
            {
                case "Name_Desc":
                    products = products.OrderByDescending(prd => prd.name);
                    break;
                case "Price":
                    products = products.OrderBy(prd => prd.price);
                    break;
                case "Price_Desc":
                    products = products.OrderByDescending(prd => prd.price);
                    break;
                default:
                    products = products.OrderBy(prd => prd.name);
                    break;
            }

            // pagination
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        #endregion

        #region Category management

        public ActionResult AddCategory()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(NewCategory newCategory)
        {
            if (ModelState.IsValid)
            {
                if (newCategory.NewPicture != null)
                {
                    // get the file's full name and extension
                    string _FileName = Path.GetFileName(newCategory.NewPicture.FileName);
                    var _Ext = Path.GetExtension(newCategory.NewPicture.FileName);

                    // check if the file provided is valid
                    if (allowedExtensions.Contains(_Ext))
                    {
                        string name = Path.GetFileNameWithoutExtension(_FileName);

                        // create a new image item and it's ID
                        ProductImage newImage = new ProductImage
                        {
                            ImageID = Guid.NewGuid()
                        };

                        // create a new filename ( name_ImageID.ext)
                        string myfile = name + "_" + newImage.ImageID + _Ext;

                        //create a complete path for saving
                        string _path = Path.Combine(Server.MapPath("~/Content/productImages"), myfile);

                        //set the image obj name and date
                        newImage.imageName = myfile;
                        newImage.UploadDate = DateTime.UtcNow;
                        newImage.imageType = _Ext;

                        _dbContext.ProductImages.Add(newImage);

                        //save the picture in the server
                        newCategory.NewPicture.SaveAs(_path);
                        newCategory.category.categoryImage = newImage.ImageID;
                    }
                    else
                    {
                        ViewBag.message = "Please choose only an Image file";
                        return View();
                    }
                }
                else // no image was provided
                {
                    newCategory.category.categoryImage = Guid.Parse("D9BC92BA-5D32-4E39-89A9-BECEA428E1C9");
                }
                // set category
                _dbContext.Categories.Add(newCategory.category);

                // save and return to the list
                _dbContext.SaveChanges();
                return RedirectToAction("Categories", "Manager");
            }
            else
            {
                ViewBag.message = "modelstate not valid";
                return View();
            }
        }


        public ActionResult EditCategory(int id)
        {
            Category category = _dbContext.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            NewCategory a = new NewCategory
            {
                category = category,
                oldPicture = _dbContext.ProductImages.Where(e => e.ImageID == category.categoryImage).FirstOrDefault()
            };
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(NewCategory newCategory)
        {
            if (ModelState.IsValid)
            {
                // check if a picture is provided
                if (newCategory.NewPicture != null)
                {
                    string _FileName = Path.GetFileName(newCategory.NewPicture.FileName);
                    var _Ext = Path.GetExtension(newCategory.NewPicture.FileName);

                    // check if the file provided is valid
                    if (allowedExtensions.Contains(_Ext))
                    {
                        //getting file name without extension  
                        string name = Path.GetFileNameWithoutExtension(_FileName);
                        string myfile, _path;

                        //update old image
                        if ((newCategory.oldPicture != null) && (newCategory.oldPicture.imageName != "noimage.jpg"))
                        {
                            // create a new filename ( name_ImageID.ext)
                            myfile = name + "_" + newCategory.oldPicture.ImageID + _Ext;

                            //create a complete path for saving
                            _path = Path.Combine(Server.MapPath("~/Content/productImages"), myfile);

                            //delete old picture
                            System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/productImages"), newCategory.oldPicture.imageName));

                            //set the image obj name and date
                            newCategory.oldPicture.imageName = myfile;
                            newCategory.oldPicture.UploadDate = DateTime.UtcNow;
                            newCategory.oldPicture.imageType = _Ext;
                            _dbContext.Entry(newCategory.oldPicture).State = EntityState.Modified;

                        }
                        // create new image
                        else
                        {
                            ProductImage newimage = new ProductImage
                            {
                                ImageID = Guid.NewGuid(),
                                imageType = _Ext,
                                UploadDate = DateTime.UtcNow
                            };
                            newimage.imageName = name + "_" + newimage.ImageID + _Ext;
                            _path = Path.Combine(Server.MapPath("~/Content/productImages"), newimage.imageName);
                            _dbContext.ProductImages.Add(newimage);
                            newCategory.category.categoryImage = newimage.ImageID;
                        }

                        //save the picture in the server
                        newCategory.NewPicture.SaveAs(_path);
                    }
                    else
                    {
                        ViewBag.message = "Please choose only an Image file";
                        return View(newCategory);
                    }
                }

                _dbContext.Entry(newCategory.category).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Categories");
            }
            ViewBag.message = "modelstate invalid";
            return View(newCategory);
        }

        public ActionResult DeleteCategory(int id)
        {
            Category Category = _dbContext.Categories.Find(id);
            if (Category == null)
            {
                return HttpNotFound();
            }
            return View(Category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedCat(int id)
        {
            Category category = _dbContext.Categories.Find(id);
            ProductImage image = _dbContext.ProductImages.Where(e => e.ImageID == category.categoryImage).FirstOrDefault();
            if (image.imageName != "noimage.jpg")
            {
                System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/productImages"), image.imageName));
                _dbContext.ProductImages.Remove(image);
            }
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            return RedirectToAction("Categories");
        }

        #endregion

        #region Admin Management

        public ActionResult NewAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewAdmin(RegisterModel registerUser)
        {
            if (ModelState.IsValid) //update it to add address and user
            {
                //check if the email is free
                bool UserExsist = _dbContext.Users
               .Any(u => u.emailAddress == registerUser.User.emailAddress); // the email is free
                if (!UserExsist)
                {
                    // give the address an id and add it to the db
                    registerUser.Address.addressID = Guid.NewGuid();
                    _dbContext.Addresses.Add(registerUser.Address);

                    // set some parameters to the user and add it to the db
                    registerUser.User.passwordSalt = GetSalt();
                    registerUser.User.passwordHash = GetHash(registerUser.password, registerUser.User.passwordSalt);
                    registerUser.User.userID = Guid.NewGuid();
                    registerUser.User.addressID = registerUser.Address.addressID;
                    registerUser.User.usertype = 2;
                    _dbContext.Users.Add(registerUser.User);

                    // save, log in and return to the home page
                    _dbContext.SaveChanges();
                    return RedirectToAction("Index", "Manager");
                }
                else
                {
                    ViewBag.message = "Email already registered, please use a different one.";
                    return View();
                }
            }
            else
            {
                ViewBag.message = "modelstate invalid";
                return View();
            }
        }

        #endregion

        #region orders

        public ActionResult Orders(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Date_Older" : ""; //switch back and forth between those two values.
            //ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_Desc" : "Price"; //switch back and forth between those two values.

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilterO = searchString;
            var orders = _dbContext.Orders.Include(c => c.User);
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(ord => ord.User.emailAddress.Contains(searchString));
            }
            // sort
            switch (sortOrder)
            {
                case "Date_Older":
                    orders = orders.OrderBy(ord => ord.checkoutTime);
                    break;
                default:
                    orders = orders.OrderByDescending(ord => ord.checkoutTime);
                    break;
            }

            // pagination
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult OrderDetails(long orderId)
        {
            OrderProducts o = new OrderProducts
            {
                order = _dbContext.Orders.Where(e => e.orderId == orderId).FirstOrDefault(),
                orderItems = _dbContext.Order_Product.Where(e => e.orderId == orderId).ToList()
            };
            return View(o);
        }

        #endregion

        #region Helping functions
        public string GetHash(string pw, string salt) // add encryption
        {
            string source = pw + salt + globalSalt;
            using (SHA512 sha512Hash = SHA512.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                return hash;
            }
        }

        public string GetSalt()
        {
            int length = 5;
            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }

        #endregion
    
    }
}