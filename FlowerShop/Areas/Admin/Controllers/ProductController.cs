using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.DBContext;
using System.IO;
using static FlowerShop.Ultils.Ultils;

namespace FlowerShop.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        // GET: Admin/Product
        public ActionResult Index(string search = "")
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            CategoryDB categoryDB = new CategoryDB();
            ProductDB productDB = new ProductDB();

            ViewBag.CategoryList = categoryDB.GetCategories();
            ViewBag.SupplierList = productDB.GetSuppliers();
            ViewBag.DiscountProducts = productDB.GetDiscountProducts();

            List<Product> products;
            ViewBag.searchvalue = search;
            if (search.Length > 0)
            {
                products = productDB.GetProducts().Where(pro => pro.Name.Contains(search)).ToList();
            }
            else
            {
                products = productDB.GetProducts();
            }

            return View(products);
        }

        public ActionResult Create()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            CategoryDB categoryDB = new CategoryDB();
            ProductDB productDB = new ProductDB();

            ViewBag.CategoryList = categoryDB.GetCategories();
            ViewBag.SupplierList = productDB.GetSuppliers();


            return View();
        }

        [HttpPost]
        public ActionResult Create(Product product, int categoryId, HttpPostedFileBase ImageFile)
        {
           
            ProductDB productDB = new ProductDB();
            string ImgFileName;

            if (!ModelState.IsValid) return View();

            if (ImageFile != null) {
                ImgFileName = UploadFile("~/Assets/ImageStorage/Products", ImageFile);
            }  else {
                ImgFileName = "no-img.png";
            }


            // Insert product to db 
            bool isSuccess = productDB.AddProduct(product, categoryId, ImgFileName);
            
            if (!isSuccess)
            {
                TempData["alert-error"] = "Thêm không thành công";
                return View();
            }

            TempData["alert-success"] = "Đã thêm thành công sản phẩm";
            return RedirectToAction("Index", "Product", new { area = "Admin" });
        }

        public ActionResult Edit(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            CategoryDB categoryDB = new CategoryDB();
            ProductDB productDB = new ProductDB();

            Product product = productDB.GetProducts().Find(p => p.Id == id);

            ViewBag.CategoryList = categoryDB.GetCategories();
            ViewBag.ProductCategory = categoryDB.GetProductCategories().Find(proCate => proCate.ProductId == product.Id);
            ViewBag.SupplierList = productDB.GetSuppliers();


            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(int id, Product product, int categoryId, HttpPostedFileBase ImageFile)
        {
            ProductDB productDB = new ProductDB();
            Product currentProduct = productDB.GetProducts().Find(p => p.Id == id);

            string ImgFileName = "";

            if (ModelState.IsValid == false) {
                ViewData["edit-error"] = "Error model";
                return View();
            }


            if (ImageFile != null)
            {  
                // Delete -> Add New  
                string fullPath = Request.MapPath("~/Assets/ImageStorage/Products/" + currentProduct.Image);

                if (System.IO.File.Exists(fullPath)) {   
                    // Delete
                    System.IO.File.Delete(fullPath);
                }
                // Add
                ImgFileName = UploadFile("~/Assets/ImageStorage/Products", ImageFile);
            }
            else
            {
                ImgFileName = currentProduct.Image;
            }

            // Insert product to db 
            bool isSuccess = productDB.UpdateProduct(product, categoryId, ImgFileName);

            if (!isSuccess)
            {
                TempData["edit-error"] = "Chỉnh sửa không thành công sản phẩm";
                return View();
            }

            TempData["alert-success"] = "Đã chỉnh sửa thành công sản phẩm";
            return RedirectToAction("Index", "Product", new { area = "Admin" });
        }

        public ActionResult Detail(int id)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            CategoryDB categoryDB = new CategoryDB();
            ProductDB productDB = new ProductDB();


            Product product = productDB.GetProducts().Find(p => p.Id == id);
            ProductCategory productCategory = categoryDB.GetProductCategories().Find(proCate => proCate.ProductId == product.Id);
            Category category = categoryDB.GetCategories().Find(cate => cate.Id == productCategory.CategoryId);
            
            ViewBag.SupplierList = productDB.GetSuppliers();
            ViewBag.DiscountProducts = productDB.GetDiscountProducts();
            ViewBag.category = category.CategoryName;

            return View(product);
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