using Supermarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Supermarket
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Session_End(object sender, EventArgs e)
        {
            if (Session["CartId"] != null)
            {
                string CartSessionKey = Session["CartId"].ToString();
                System.Diagnostics.Debug.WriteLine(CartSessionKey);
                if (!CartSessionKey.Contains("@"))
                {
                    _dbContext.Carts.RemoveRange(_dbContext.Carts.Where(asdf => asdf.CartId == CartSessionKey));
                    _dbContext.SaveChanges();
                }
            }
            System.Diagnostics.Debug.WriteLine("Session_End");
        }
    }
}
