using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Supermarket.Models;
using PagedList;

namespace Supermarket.Controllers
{
    public class ShopController : Controller
    {
        private SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();

        // GET: Shop
        public ActionResult Index(string searchString, string sortOption, int category = 0, int page = 1)
        {
            int pageSize = 2;


            ViewBag.search = searchString;
            ViewBag.sort = sortOption;
            ViewBag.catg = category;


            var products = _dbContext.Products.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.name.ToLower().Contains(searchString));
            }
            if ((category != 0) && (_dbContext.Categories.Where(e => e.categoryID == category).FirstOrDefault() != null))
            {
                products = products.Where(p => p.category == category);
            }
            switch (sortOption)
            {
                case "name_acs":
                    products = products.OrderBy(p => p.name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.name);
                    break;
                case "price_acs":
                    products = products.OrderBy(p => p.price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.price);
                    break;
                case "stock_acs":
                    products = products.OrderBy(p => p.stock);
                    break;
                case "stock_desc":
                    products = products.OrderByDescending(p => p.stock);
                    break;
                default:
                    products = products.OrderBy(p => p.name);
                    break;

            }
            ViewBag.category = new SelectList(_dbContext.Categories, "categoryID", "categoryName");
            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("_ProductList", products.ToPagedList(page, pageSize))
                : View(products.ToPagedList(page, pageSize));
        }

        // GET: Shop/Details/5
        public ActionResult Details(Guid? id)
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

        //// GET: Shop/Create
        //public ActionResult Create()
        //{
        //    ViewBag.category = new SelectList(db.Categories, "categoryID", "categoryName");
        //    return View();
        //}

        //// POST: Shop/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "productID,name,price,description,stock,imageURL,category")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        product.productID = Guid.NewGuid();
        //        db.Products.Add(product);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.category = new SelectList(db.Categories, "categoryID", "categoryName", product.category);
        //    return View(product);
        //}

        //// GET: Shop/Edit/5
        //public ActionResult Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Product product = db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.category = new SelectList(db.Categories, "categoryID", "categoryName", product.category);
        //    return View(product);
        //}

        //// POST: Shop/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "productID,name,price,description,stock,imageURL,category")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(product).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.category = new SelectList(db.Categories, "categoryID", "categoryName", product.category);
        //    return View(product);
        //}

        //// GET: Shop/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Product product = db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(product);
        //}

        //// POST: Shop/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Product product = db.Products.Find(id);
        //    db.Products.Remove(product);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
