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
        private SupermarketEntitiesDB db = new SupermarketEntitiesDB();

        // GET: Shop
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_Desc" : ""; //switch back and forth between those two values.
            ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_Desc" : "Price";//switch back and forth between those two values.
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var products = db.Products.Include(p => p.Category1);
            // search
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(prd => prd.name.Contains(searchString)
                                       || prd.description.Contains(searchString)
                                       || prd.Category1.categoryName.Contains(searchString));
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
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        // GET: Shop/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
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
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
