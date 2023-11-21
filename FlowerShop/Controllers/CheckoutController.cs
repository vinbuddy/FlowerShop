using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FlowerShop.Models;
using FlowerShop.DBContext;
using System.Configuration;
using System.Data.SqlClient;

namespace FlowerShop.Controllers
{
    public class CheckoutController : Controller
    {
        public CheckoutController()
        {
            // Pass data to layout
            CategoryDB categoryDB = new CategoryDB();
            this.ViewData["categories"] = categoryDB.GetCategories();
        }

        // GET: Checkout
        public ActionResult ShowCheckout()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            ViewBag.ItemSelected = TempData["ItemSelected"];
            TempData.Keep("ItemSelected");

            var itemSelected = TempData["ItemSelected"] as IEnumerable<int>;
            int userId = Convert.ToInt32(Session["UserId"]);

            // ItemSelected -> Get cart item 
            CartDB cartDB = new CartDB();
            ViewBag.selectedItems = cartDB.GetCartItems(userId).Where(item => itemSelected.Any(itemSelect => itemSelect == item.ProductId)).ToList();
            List<CartItem> orderItems = cartDB.GetCartItems(userId).Where(item => itemSelected.Any(itemSelect => itemSelect == item.ProductId)).ToList();

            TempData["OrderItems"] = orderItems;
            TempData.Keep("OrderItems");

            return View();
        }

        [HttpPost]
        public ActionResult CreateOrder(Order order, string FullName, string PhoneNumber, string Address, string City, string District, string Ward)
        {

            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(City) || string.IsNullOrEmpty(District) || string.IsNullOrEmpty(Ward))
            {
                return RedirectToAction("ShowCheckout");
            }

            List<CartItem> orderItems = TempData["OrderItems"] as List<CartItem>;

            ShippingOrder orderShippingData = new ShippingOrder()
            {
                FullName = FullName,
                PhoneNumber = PhoneNumber,
                Address = Address,
                City = City,
                District = District,
                Ward = Ward
            };


            OrderDB orderDB = new OrderDB();

            try
            {
                var orderCreated = orderDB.CreateOrder(orderShippingData, order, orderItems);
                bool isSuccess = orderCreated.Item1;
                int orderId = orderCreated.Item2;

                if (isSuccess)
                {
                    // Send Email
                    UserDB userDB = new UserDB();

                    Order currentOrder = orderDB.GetOrders().Find(o => o.Id == orderId);
                    List<OrderDetail> currentOrderItems = new OrderDB().GetOrderDetails(orderId);
                    User user = userDB.GetUsers().Find(u => u.Id == order.UserId);
                    ShippingOrder shippingOrder = orderDB.GetShippingOrder(orderId);


                    string orderProducts = "";
                    decimal totalPrice = decimal.Zero;

                    foreach (var orderProduct in currentOrderItems)
                    {
                        totalPrice += orderProduct.TotalOrderPrice;

                        orderProducts += "<tr>";
                        orderProducts += "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif;word-wrap:break-word\">" + orderProduct.ProductName + "</td>";
                        orderProducts += "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif\">" + orderProduct.Quantity + "</td>";
                        orderProducts += "<td style=\"color:#636363;border:1px solid #e5e5e5;padding:12px;text-align:left;vertical-align:middle;font-family:'Helvetica Neue',Helvetica,Roboto,Arial,sans-serif\">" + @String.Format("{0:N}", orderProduct.TotalOrderPrice) + "</td>";
                        orderProducts += "<tr>";
                    }

                    string customerContent = System.IO.File.ReadAllText(Server.MapPath("~/Content/Templates/CustomerEmail.html"));
                    customerContent = customerContent.Replace("{{OrderId}}", currentOrder.Id.ToString());
                    customerContent = customerContent.Replace("{{ReceiveDate}}", currentOrder.ReceiveDate);
                    customerContent = customerContent.Replace("{{ReceiveTime}}", currentOrder.ReceiveTime);
                    customerContent = customerContent.Replace("{{OrderDate}}", currentOrder.OrderDate);
                    customerContent = customerContent.Replace("{{OrderProducts}}", orderProducts);
                    customerContent = customerContent.Replace("{{TotalPrice}}", @String.Format("{0:N}", totalPrice));
                    customerContent = customerContent.Replace("{{ShippingCost}}", @String.Format("{0:N}", currentOrder.ShippingCost));
                    customerContent = customerContent.Replace("{{TotalPayment}}", @String.Format("{0:N}", currentOrder.TotalPayment));
                    customerContent = customerContent.Replace("{{FullName}}", shippingOrder.FullName);
                    customerContent = customerContent.Replace("{{PhoneNumber}}", shippingOrder.PhoneNumber);
                    customerContent = customerContent.Replace("{{Address}}", shippingOrder.Address);
                    customerContent = customerContent.Replace("{{Ward}}", shippingOrder.Ward);
                    customerContent = customerContent.Replace("{{District}}", shippingOrder.District);
                    customerContent = customerContent.Replace("{{City}}", shippingOrder.City);

                    FlowerShop.Ultils.Ultils.SendEmail("Flower Shop", "Đơn hàng #" + currentOrder.Id.ToString(), customerContent.ToString(), user.Email);


                    // Admin
                    string adminContent = System.IO.File.ReadAllText(Server.MapPath("~/Content/Templates/CustomerEmail.html"));
                    adminContent = adminContent.Replace("{{OrderId}}", currentOrder.Id.ToString());
                    adminContent = adminContent.Replace("{{OrderDate}}", currentOrder.OrderDate);
                    adminContent = adminContent.Replace("{{ReceiveDate}}", currentOrder.ReceiveDate);
                    adminContent = adminContent.Replace("{{ReceiveTime}}", currentOrder.ReceiveTime);
                    adminContent = adminContent.Replace("{{OrderProducts}}", orderProducts);
                    adminContent = adminContent.Replace("{{TotalPrice}}", @String.Format("{0:N}", totalPrice));
                    adminContent = adminContent.Replace("{{ShippingCost}}", @String.Format("{0:N}", currentOrder.ShippingCost));
                    adminContent = adminContent.Replace("{{TotalPayment}}", @String.Format("{0:N}", currentOrder.TotalPayment));
                    adminContent = adminContent.Replace("{{FullName}}", shippingOrder.FullName);
                    adminContent = adminContent.Replace("{{PhoneNumber}}", shippingOrder.PhoneNumber);
                    adminContent = adminContent.Replace("{{Address}}", shippingOrder.Address);
                    adminContent = adminContent.Replace("{{Ward}}", shippingOrder.Ward);
                    adminContent = adminContent.Replace("{{District}}", shippingOrder.District);
                    adminContent = adminContent.Replace("{{City}}", shippingOrder.City);
                    adminContent = adminContent.Replace("{{Email}}", user.Email);


                    FlowerShop.Ultils.Ultils.SendEmail("Flower Shop", "Đơn hàng #" + currentOrder.Id.ToString(), adminContent.ToString(), ConfigurationManager.AppSettings["EmailAdmin"]);

                    TempData["OrderId"] = currentOrder.Id;
                    TempData["OrderStatus"] = currentOrder.Status;
                    TempData["OrderDate"] = currentOrder.OrderDate;


                    return RedirectToAction("SuccessCheckout");
                }

            }
            catch (SqlException sqlEx)
            {
                TempData["error-message"] = sqlEx.Message;
                return RedirectToAction("ShowCheckout");

            }
            catch (Exception ex)
            {
                TempData["error-message"] = ex.Message;
                return RedirectToAction("ShowCheckout");
            }

            return RedirectToAction("ShowCheckout");

        }

        public ActionResult SuccessCheckout()
        {
            return View();
        }
    }
}