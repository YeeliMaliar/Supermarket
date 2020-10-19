using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using Supermarket.ViewModels;
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
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult About()
        {
            return View();
        }

        // simulating timeout.
        public ActionResult Abandon()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}