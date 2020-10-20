using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using Supermarket.Models;
using PagedList;

namespace Supermarket.Controllers
{
    public class StoreController : Controller
    {
        
        private readonly SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();

        // GET: Store
        
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? category, int? page)
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
            ViewBag.CurrentFilter = searchString;
            var products = _dbContext.Products.Include(p => p.Category1);
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
        
        
        public ActionResult Product(Guid id)
        {
            Product prod = _dbContext.Products.Where(e => e.productID == id).FirstOrDefault();
            return View(prod);
        }


    }
}