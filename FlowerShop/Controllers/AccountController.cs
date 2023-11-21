using FlowerShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.DBContext;
using FlowerShop.Models;




namespace FlowerShop.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterVM registerInfo)
        {
            UserDB userDB = new UserDB();
            RoleDB roleDB = new RoleDB();


            if (!ModelState.IsValid)
            {
                return View();
            }

            // Check exist
            if (userDB.isExisted(registerInfo.UserName, registerInfo.Email))
            {
                ViewBag.AccountExistedError = "This account already exists";
                return View();
            }

            // Create account
            var user = userDB.CreateUser(registerInfo);
            bool success = user.Item1;
            int newUserId = user.Item2;


            // Set role customer
            if (success)
            {
                roleDB.AddToRole("Customer", newUserId);
            }

            // Create session
            Session["UserName"] = registerInfo.UserName;
            Session["UserId"] = newUserId;
            Session["User"] = registerInfo;



            // Redirect home page
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User loginInfo)
        {
            UserDB userDB = new UserDB();
            RoleDB roleDB = new RoleDB();

            var userLogin = userDB.GetUsers().Find(user => user.Email.Equals(loginInfo.Email) && user.Password.Equals(loginInfo.Password));
            
            if (userLogin == null)
            {
                ViewBag.Notification = "Wrong email or password";
                return View();
            }

            // Admin login
            bool isAdmin = roleDB.IsInRole(userLogin.Id, "Admin");
            if (isAdmin)
            {
                Session["User"] = userLogin;
                Session["RoleName"] = "Admin";

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            Session["Username"] = userLogin.UserName;
            Session["UserId"] = userLogin.Id;
            Session["User"] = userLogin;

            return RedirectToAction("Index", "Home");        
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}