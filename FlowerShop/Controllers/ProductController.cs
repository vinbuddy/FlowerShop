using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.DBContext;
using FlowerShop.Models;

namespace FlowerShop.Controllers
{
    public class ProductController : Controller
    {
        public ProductController()
        {
            // Pass data to layout
            CategoryDB categoryDB = new CategoryDB();
            this.ViewData["categories"] = categoryDB.GetCategories();

        }

        // Localhost:3223/Home/Shop/1
        public ActionResult Shop(int id, string search = "", string sortByPrice = "desc", int page = 1)
        {
            CategoryDB categoryDB = new CategoryDB();
            ProductDB productDB = new ProductDB();

            ViewBag.categories = categoryDB.GetCategories();
            ViewBag.discountProducts = productDB.GetDiscountProducts();
            ViewBag.search = search;
            ViewBag.currentCateId = id;
            ViewBag.currentSortByPrice = sortByPrice;

            List<Product> products;
            // All Products
            if (id == 0)
            {
                products = productDB.GetProducts().Where(pro => pro.Name.Contains(search)).ToList();
                products = sortByPrice == "desc" ? products.OrderBy(prod => prod.Price).ToList() : products.OrderByDescending(prod => prod.Price).ToList();
               
            } else
            {
                List<ProductCategory> productCategories = categoryDB.GetProductCategories().Where(p => p.CategoryId == id).ToList();
                products = productDB.GetProducts().Where(pro => productCategories.Any(procate => procate.ProductId == pro.Id) && pro.Name.Contains(search)).ToList();

                //var graduationFlowers = products.Where(pro => graduationFlowerCategories.Take(8).Any(grad => grad.ProductId == pro.Id));

                //products = productDB.GetProducts().Where(pro => pro.Id.Equals(id) && pro.Name.Contains(search)).ToList();
                products = sortByPrice == "desc" ? products.OrderBy(prod => prod.Price).ToList() : products.OrderByDescending(prod => prod.Price).ToList();

            }

            // Pagination 
            int numRecordPerPage = 9;
            int recordSize = products.Count;
            int pageSize = Convert.ToInt32(Math.Ceiling((decimal)recordSize / numRecordPerPage));
            // page 1: get 9 record - page 2: skip 9 - page 2: skip 18
            int numRecordToSkip = (page - 1) * numRecordPerPage;

            ViewBag.currentPage = page;
            ViewBag.pageSize = pageSize;


            products = products.Skip(numRecordToSkip).Take(numRecordPerPage).ToList();

            return View(products);
        }

        // Localhost:3223/Home/Detail/12
        public ActionResult Detail(int id)
        {
            ProductDB productDB = new ProductDB();
            CategoryDB categoryDB = new CategoryDB();
            Random rnd = new Random();

            ViewBag.suggestionProducts = productDB.GetProducts().OrderBy(x => rnd.Next()).Take(6);
            ViewBag.discountProducts = productDB.GetDiscountProducts();
            Product product = productDB.GetProducts().Find(pro => pro.Id == id);

            return View(product);
        }

        public ActionResult DiscountProduct()
        {
            ProductDB productDB = new ProductDB();

            ViewBag.products = productDB.GetProducts();
            ViewBag.discountProducts = productDB.GetDiscountProducts();

            return View();
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            ProductDB productDB = new ProductDB();
            Product product = productDB.GetProducts().Find(p => p.Id == id);

            // Delete
            bool isSuccess = productDB.DeleteProduct(id);

            // Delete Img
            if (isSuccess)
            {
                string fullPath = Request.MapPath("~/Assets/ImageStorage/Products/" + product.Image);

                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);

                TempData["alert-success"] = "Đã xóa thành công sản phẩm";
                return RedirectToAction("Index", "Product", new { area = "Admin" });
            }

            TempData["alert-error"] = "Xóa không thành công sản phẩm";
            return View();
        }

        [HttpPost]
        public ActionResult CreateDiscount(int id, decimal newPrice)
        {
            ProductDB productDB = new ProductDB();
            bool isSuccess = productDB.CreateDiscount(id, newPrice);

            if (isSuccess)
            {
                TempData["alert-success"] = "Đã tạo giảm giá thành công";
                return RedirectToAction("Index", "Product", new { area = "Admin" });
            }

            TempData["alert-error"] = "Tạo giảm giá không thành công";
            return View();

        }



        [HttpPost]
        public ActionResult DeleteDiscount(int id)
        {
            ProductDB productDB = new ProductDB();
            bool isSuccess = productDB.DeleteDiscount(id);

            if (isSuccess)
            {
                TempData["alert-success"] = "Đã xóa giảm giá thành công";
                return RedirectToAction("Index", "Product", new { area = "Admin" });
            }

            TempData["alert-error"] = "Xóa giảm giá không thành công";
            return View();
        }
    }
}