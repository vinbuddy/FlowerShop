using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FlowerShop.ViewModel;
using FlowerShop.Models;

using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace FlowerShop.DBContext
{
    public class CartDB
    {
        public string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
       

        public List<CartItem> GetCartItems(int userId)
        {

            List<CartItem> cartItemList = new List<CartItem>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT CartItems.*, Products.Name, Products.Image, Products.Price, DiscountProducts.DiscountPrice, Products.Price * CartItems.Quantity AS Total, DiscountProducts.DiscountPrice * CartItems.Quantity AS TotalDiscount FROM CartItems FULL OUTER JOIN Products ON Products.Id = CartItems.ProductId FULL OUTER JOIN DiscountProducts ON DiscountProducts.ProductId = CartItems.ProductId WHERE CartItems.UserId = @userId;";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@userId", userId);


            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                CartItem cartItem = new CartItem();

                cartItem.Id = Convert.ToInt32(dataReader["Id"].ToString());
                cartItem.ProductId = Convert.ToInt32(dataReader["ProductId"].ToString());
                cartItem.Quantity = Convert.ToInt32(dataReader["Quantity"].ToString());
                cartItem.UserId = Convert.ToInt32(dataReader["UserId"].ToString());
                cartItem.ProductName = dataReader["Name"].ToString();
                cartItem.ProductImage = dataReader["Image"].ToString();

                cartItem.ProductPrice = decimal.Parse(dataReader["Price"].ToString());
                cartItem.ProductDiscountPrice = dataReader["DiscountPrice"] == DBNull.Value ? 0 : decimal.Parse(dataReader["DiscountPrice"].ToString());
                cartItem.TotalPriceItem = decimal.Parse(dataReader["Total"].ToString());
                cartItem.TotalDiscount = dataReader["TotalDiscount"] == DBNull.Value ? 0 : decimal.Parse(dataReader["TotalDiscount"].ToString());



                cartItemList.Add(cartItem);
            }

            connection.Close();


            return cartItemList;

        }

        public bool AddItem(int userId, int productId, int quantity)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "INSERT INTO CartItems(ProductId, UserId, Quantity) VALUES( @productId, @userId, @quantity);";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@quantity", quantity);

            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result > 0;
        }

        public bool UpdateQuantity(int userId, int productId, int quantity)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "UPDATE CartItems SET Quantity = Quantity + @quantity WHERE ProductId = @productId AND UserId = @userId;";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@quantity", quantity);

            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result > 0;
        }

        public bool UpdateIncreaseQuantity(int userId, int productId)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "UPDATE CartItems SET Quantity = Quantity + 1 WHERE ProductId = @productId AND UserId = @userId;";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@userId", userId);

            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result > 0;
        }

        public bool UpdateDecreaseQuantity(int userId, int productId)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "UPDATE CartItems SET Quantity = Quantity - 1 WHERE ProductId = @productId AND UserId = @userId;";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@userId", userId);

            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result > 0;
        }

        public bool DeleteCartItem(int userId, int productId)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();


            cmd.CommandText = "DELETE FROM CartItems WHERE ProductId = @productId AND UserId = @userId;";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@productId", productId);
            cmd.Parameters.AddWithValue("@userId", userId);


            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();


            return result > 0;
        }
    }
}