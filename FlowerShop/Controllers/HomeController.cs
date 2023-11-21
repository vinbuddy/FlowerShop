using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.DBContext;
using FlowerShop.Models;


namespace FlowerShop.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            // Pass data to layout
            CategoryDB categoryDB = new CategoryDB();
            this.ViewData["categories"] = categoryDB.GetCategories();
        }
        // GET: Home
        public ActionResult Index()
        {
            ProductDB productDB = new ProductDB();
            CategoryDB categoryDB  = new CategoryDB();


            ViewBag.products = productDB.GetProducts();
            ViewBag.categories = categoryDB.GetCategories();
            ViewBag.productCategories = categoryDB.GetProductCategories();
            ViewBag.discountProducts = productDB.GetDiscountProducts();

            return View();
        }

    }
}