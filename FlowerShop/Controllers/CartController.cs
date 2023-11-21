using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FlowerShop.Models;
using FlowerShop.DBContext;

namespace FlowerShop.Controllers
{
    

    public class CartController : Controller
    {
        public CartController()
        {
            // Pass data to layout
            CategoryDB categoryDB = new CategoryDB();
            this.ViewData["categories"] = categoryDB.GetCategories();
        }
        // GET: Cart
        public ActionResult ShowCart()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SelectItem(CartItemSelected data)
        {
            TempData["ItemSelected"] = data.ItemSeleted;
            return Json(new { redirectToUrl = Url.Action("ShowCheckout", "Checkout") });
        }

    }
}