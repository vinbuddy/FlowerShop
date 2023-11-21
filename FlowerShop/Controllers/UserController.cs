using FlowerShop.DBContext;
using FlowerShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using static FlowerShop.Ultils.Ultils;

namespace FlowerShop.Controllers
{
    public class UserController : Controller
    {
        public UserController()
        {
            // Pass data to layout
            CategoryDB categoryDB = new CategoryDB();
            this.ViewData["categories"] = categoryDB.GetCategories();
        }
        // GET: User
        public new ActionResult Profile()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditProfile(User userInfo, HttpPostedFileBase ImageFile)
        {
            string ImgFileName;
            User currentUser = new UserDB().GetUsers().Find(u => u.Id == userInfo.Id);

            if (ImageFile != null )
            {
                // Delete -> Add New
                string fullPath = Request.MapPath("~/Assets/ImageStorage/Avatars/" + currentUser.Avatar);

                if (System.IO.File.Exists(fullPath) && currentUser.Avatar != "DefaultUserAvatar.jpg")
                {
                    // Delete
                    System.IO.File.Delete(fullPath);
                }
                // Add
                ImgFileName = UploadFile("~/Assets/ImageStorage/Avatars", ImageFile);
            }
            else
            {
                ImgFileName = currentUser.Avatar;
            }

            // Insert product to db 
            bool isSuccess = new UserDB().UpdateInfo(userInfo, ImgFileName);

            if (isSuccess)
            {
                Session["UserName"] = userInfo.UserName;
                Session["User"] = new UserDB().GetUsers().Find(u => u.Id == userInfo.Id);
                return RedirectToAction("Profile", "User");
            }

            return View();
        }


        public ActionResult Purchase()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            OrderDB orderDB = new OrderDB();
            UserDB userDB = new UserDB();

            int userId = Convert.ToInt32(Session["UserId"]);

            List<Order> orders = orderDB.GetOrders().Where(order => order.UserId == userId).ToList();
            List<List<OrderDetail>> orderList = new List<List<OrderDetail>>();
            foreach (var order in orders)
            {

                List<OrderDetail> orderItems = orderDB.GetOrderDetails(order.Id);
                orderList.Add(orderItems);
            }
            ViewBag.orderItemList = orderList;
            return View(orders);
        }

        public ActionResult CancelOrder(int id)
        {
            OrderDB orderDB = new OrderDB();
            bool isSuccess = orderDB.UpdateOrderStatus(id, "Đã hủy");

            if (isSuccess)
            {
                return RedirectToAction("Purchase", "User", new { area = "" });
            }

            return RedirectToAction("Purchase", "User", new { area = "" });
        }

    }
}