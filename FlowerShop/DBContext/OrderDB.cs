using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Configuration;
using FlowerShop.Models;
using System.Data.SqlClient;

namespace FlowerShop.DBContext
{
    public class OrderDB
    {
        public string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public Tuple<bool, int> CreateOrder(ShippingOrder shippingOrder, Order order, List<CartItem> orderItems)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            string orderCommand = "INSERT INTO Orders(UserId, TotalPayment, OrderDate, ReceiveDate, ReceiveTime, Note, ShippingCost) VALUES(@UserId, @TotalPayment, @OrderDate, @ReceiveDate, @ReceiveTime, @Note, @ShippingCost); SELECT SCOPE_IDENTITY();";
            string orderDetailCommand = "INSERT INTO OrderDetails(OrderId, ProductId, Quantity, OrderPrice) VALUES(@OrderId, @ProductId, @Quantity, @OrderPrice)";
            string shippingOrderCommand = "INSERT INTO ShippingOrders(OrderId, PhoneNumber, Address, City, District, Ward, FullName) VALUES(@OrderId, @PhoneNumber, @Address, @City, @District, @Ward, @FullName)";

            cmd.CommandText = orderCommand;
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@UserId", order.UserId);
            cmd.Parameters.AddWithValue("@TotalPayment", order.TotalPayment);
            cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@ReceiveDate", order.ReceiveDate);
            cmd.Parameters.AddWithValue("@ReceiveTime", order.ReceiveTime);
            cmd.Parameters.AddWithValue("@Note", string.IsNullOrEmpty(order.Note) ? "" : order.Note);
            cmd.Parameters.AddWithValue("@ShippingCost", order.ShippingCost);


            connection.Open();

            // Insert Order table -> return id
            int orderId = Convert.ToInt32(cmd.ExecuteScalar());


            // OrderDetail
            cmd.CommandText = orderDetailCommand;
            cmd.Parameters.Add("@OrderId", SqlDbType.Int);
            cmd.Parameters.Add("@ProductId", SqlDbType.Int);
            cmd.Parameters.Add("@Quantity", SqlDbType.Int);
            cmd.Parameters.Add("@OrderPrice", SqlDbType.Decimal);


            foreach (CartItem orderItem in orderItems)
            {
                cmd.Parameters["@OrderId"].Value = orderId;
                cmd.Parameters["@ProductId"].Value = orderItem.ProductId;
                cmd.Parameters["@Quantity"].Value = orderItem.Quantity;
                cmd.Parameters["@OrderPrice"].Value = orderItem.ProductDiscountPrice != 0 ? orderItem.ProductDiscountPrice : orderItem.ProductPrice;

                cmd.ExecuteNonQuery();
            }

            // Shipping
            cmd.CommandText = shippingOrderCommand;
            cmd.Parameters.AddWithValue("@PhoneNumber", shippingOrder.PhoneNumber);
            cmd.Parameters.AddWithValue("@Address", shippingOrder.Address);
            cmd.Parameters.AddWithValue("@City", shippingOrder.City);
            cmd.Parameters.AddWithValue("@District", shippingOrder.District);
            cmd.Parameters.AddWithValue("@Ward", shippingOrder.Ward);
            cmd.Parameters.AddWithValue("@FullName", shippingOrder.FullName);


            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return Tuple.Create(result > 0, orderId);
        }

        public List<Order> GetOrders()
        {
            List<Order> orderList = new List<Order>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM Orders";
            cmd.Connection = connection;

            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                Order order = new Order();

                order.Id = Convert.ToInt32(dataReader["Id"].ToString());
                order.UserId = Convert.ToInt32(dataReader["UserId"].ToString());
                order.OrderDate = DateTime.Parse(dataReader["OrderDate"].ToString()).ToString("dd/MM/yyyy");
                order.ReceiveDate = DateTime.Parse(dataReader["ReceiveDate"].ToString()).ToString("dd/MM/yyyy");
                order.ReceiveTime = dataReader["ReceiveTime"].ToString();
                order.TotalPayment = decimal.Parse(dataReader["TotalPayment"].ToString());
                order.ShippingCost = decimal.Parse(dataReader["ShippingCost"].ToString());
                order.Status = dataReader["Status"].ToString();
                order.Note = dataReader["Note"].ToString();


                orderList.Add(order);
            }

            connection.Close();


            return orderList;
        }

        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            List<OrderDetail> orderDetailList = new List<OrderDetail>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT OrderDetails.*, Products.Name AS ProductName, Products.Image AS ProductImage, OrderDetails.OrderPrice * OrderDetails.Quantity AS TotalOrderPrice FROM OrderDetails FULL OUTER JOIN Products ON Products.Id = OrderDetails.ProductId WHERE OrderDetails.OrderId = @orderId";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@orderId", orderId);


            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                OrderDetail orderDetail = new OrderDetail();

                orderDetail.Id = Convert.ToInt32(dataReader["Id"].ToString());
                orderDetail.OrderId = Convert.ToInt32(dataReader["OrderId"].ToString());
                orderDetail.ProductId = Convert.ToInt32(dataReader["ProductId"].ToString());
                orderDetail.Quantity = Convert.ToInt32(dataReader["Quantity"].ToString());
                orderDetail.ProductName = dataReader["ProductName"].ToString();
                orderDetail.ProductImage = dataReader["ProductImage"].ToString();
                orderDetail.TotalOrderPrice = decimal.Parse(dataReader["TotalOrderPrice"].ToString());
                orderDetail.OrderPrice = decimal.Parse(dataReader["OrderPrice"].ToString());


                orderDetailList.Add(orderDetail);
            }

            connection.Close();


            return orderDetailList;
        }

        public ShippingOrder GetShippingOrder(int orderId)
        {
            ShippingOrder shippingOrder = new ShippingOrder();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM ShippingOrders WHERE OrderId = @orderId";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@orderId", orderId);

            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {

                shippingOrder.Id = Convert.ToInt32(dataReader["Id"].ToString());
                shippingOrder.OrderId = Convert.ToInt32(dataReader["OrderId"].ToString());
                shippingOrder.PhoneNumber = dataReader["PhoneNumber"].ToString();
                shippingOrder.FullName = dataReader["FullName"].ToString();
                shippingOrder.City = dataReader["City"].ToString();
                shippingOrder.District = dataReader["District"].ToString();
                shippingOrder.Ward = dataReader["Ward"].ToString();
                shippingOrder.Address = dataReader["Address"].ToString();

            }

            connection.Close();


            return shippingOrder;
        }



        public List<OrderStatus> GetOrderStatus()
        {
            List<OrderStatus> orderStatusList = new List<OrderStatus>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT Orders.Status AS StatusName, COUNT(*) AS StatusQuantity FROM Orders GROUP BY Orders.Status";
            cmd.Connection = connection;

            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                OrderStatus orderStatus = new OrderStatus();

                orderStatus.StatusQuantity = Convert.ToInt32(dataReader["StatusQuantity"].ToString());
                orderStatus.StatusName = dataReader["StatusName"].ToString();

                orderStatusList.Add(orderStatus);
            }

            connection.Close();


            return orderStatusList;
        }


        public bool UpdateOrderStatus(int orderId, string status)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "UPDATE Orders SET Status = @status WHERE Id = @orderId;";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@orderId", orderId);
            cmd.Parameters.AddWithValue("@status", status);


            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();


            return result > 0;
        }
    }
}