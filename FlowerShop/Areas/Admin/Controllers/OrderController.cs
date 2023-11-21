using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FlowerShop.Models;
using FlowerShop.DBContext;


namespace FlowerShop.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order
        public ActionResult Index(string status = "")
        {
            OrderDB orderDB = new OrderDB();
            UserDB userDB = new UserDB();
            List<Order> orders;

            switch(status)
            {
                case "pending":
                    orders = orderDB.GetOrders().Where(o => o.Status == "Đang chờ").ToList();
                    break;
                case "success":
                    orders = orderDB.GetOrders().Where(o => o.Status == "Hoàn thành").ToList();
                    break;
                case "shipping":
                    orders = orderDB.GetOrders().Where(o => o.Status == "Đang giao").ToList();
                    break;
                case "processing":
                    orders = orderDB.GetOrders().Where(o => o.Status == "Đang xử lý").ToList();
                    break;
                case "cancel":
                    orders = orderDB.GetOrders().Where(o => o.Status == "Đã hủy").ToList();
                    break;
                default:
                    orders = orderDB.GetOrders();
                    break;
            }


            ViewBag.users = userDB.GetUsers();
            ViewBag.orderStatusList = orderDB.GetOrderStatus();

            return View(orders);
        }

        public ActionResult OrderDetail(int id)
        {
            OrderDB orderDB = new OrderDB();
            UserDB userDB = new UserDB();

            Order order = orderDB.GetOrders().Find(o => o.Id == id);

            List<OrderDetail> orderItems = new OrderDB().GetOrderDetails(id);
            ViewBag.order = order;
            ViewBag.user = userDB.GetUsers().Find(u => u.Id == order.UserId);
            ViewBag.shippingOrders = orderDB.GetShippingOrder(id);


            return View(orderItems);
        }

        [HttpPost]
        public ActionResult UpdateOrderStatus(int id, string status)
        {
            OrderDB orderDB = new OrderDB();
            bool isSuccess = orderDB.UpdateOrderStatus(id, status);

            if (isSuccess)
            {
                return RedirectToAction("OrderDetail", "Order", new { id = id, area = "Admin" });
            }

            return RedirectToAction("OrderDetail", "Order", new { id = id, area = "Admin" });

        }
    }
}