using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Supermarket.Models;
using System.Dynamic;
using System.Text;
using System.Security.Cryptography;

namespace Supermarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult About()
        {

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}