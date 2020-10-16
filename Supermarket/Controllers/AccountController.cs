using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Supermarket.Models;
using Supermarket.ViewModels;
using System.Web.Mvc;
using System.Dynamic;
using System.Text;
using System.Security.Cryptography;

namespace Supermarket.Controllers
{
    [CustomAuthorize]
    public class AccountController : Controller
    {
        private readonly SupermarketEntitiesDB _dbContext = new SupermarketEntitiesDB();

        private string globalSalt = "N2Pzd1tFN1";

        // GET: Account

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel LoginModel)
        {
            if (ModelState.IsValid)
            {
                bool UserExsist = _dbContext.Users
               .Any(u => u.emailAddress == LoginModel.email);
                if (UserExsist)
                {
                    User TheUser = _dbContext.Users.FirstOrDefault(u => u.emailAddress == LoginModel.email);
                    string hash = getHash(LoginModel.password, TheUser.passwordSalt);
                    if (hash == TheUser.passwordHash)
                    {
                        FormsAuthentication.SetAuthCookie(LoginModel.email, false);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "invalid Email or Password");
            return View();
        }


        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel registerUser)
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
                    registerUser.User.passwordSalt = getSalt();
                    registerUser.User.passwordHash = getHash(registerUser.password, registerUser.User.passwordSalt);
                    registerUser.User.userID = Guid.NewGuid();
                    registerUser.User.addressID = registerUser.Address.addressID;
                    registerUser.User.usertype = 1;
                    _dbContext.Users.Add(registerUser.User);

                    // save, log in and return to the home page
                    _dbContext.SaveChanges();
                    FormsAuthentication.SetAuthCookie(registerUser.User.emailAddress, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.message = "Email already registered, please use a different one.";
                    return View();
                }
            }
            else
            {
                //ViewBag.message = "";
                return View();
            }
        }


        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


        public string getHash(string pw, string salt) // add encryption
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

        public string getSalt()
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
    }
}