using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using FlowerShop.Models;

namespace FlowerShop.DBContext
{
    public class ProductDB
    {
        public string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public List<Product> GetProducts()
        {
            List<Product> productList = new List<Product>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM Products";
            cmd.Connection = connection;

            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                Product product = new Product();

                product.Id = Convert.ToInt32(dataReader["Id"].ToString());
                product.SupplierId = Convert.ToInt32(dataReader["SupplierId"].ToString());
                product.Name = dataReader["Name"].ToString();
                product.Quantity = dataReader["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["Quantity"].ToString());
                product.Price = decimal.Parse(dataReader["Price"].ToString());
                product.Status = dataReader["Status"].ToString();
                product.Description = dataReader["Description"].ToString();
                product.Image = dataReader["Image"].ToString();


                productList.Add(product);
            }

            connection.Close();


            return productList;
        }

        public List<Supplier> GetSuppliers()
        {
            List<Supplier> supplierList = new List<Supplier>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM Suppliers";
            cmd.Connection = connection;

            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                Supplier supplier = new Supplier();

                supplier.Id = Convert.ToInt32(dataReader["Id"].ToString());
                supplier.Name = dataReader["Name"].ToString();
                supplier.PhoneNumber = dataReader["PhoneNumber"].ToString();
                supplier.Address = dataReader["Address"].ToString();


                supplierList.Add(supplier);
            }

            connection.Close();

            return supplierList;
        }

        public bool AddProduct(Product product, int categoryId, string ImageFileName)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            string productsCommand = "INSERT INTO Products(SupplierId, Name, Price, Quantity, Status, Description, Image) VALUES(@SupplierId, @Name, @Price, @Quantity, @Status, @Description, @Image); SELECT SCOPE_IDENTITY();";
            string productCategoriesCommand = "INSERT INTO ProductCategories(ProductId, CategoryId) VALUES(@ProductId, @CategoryId)";

            cmd.CommandText = productsCommand;
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@SupplierId", product.SupplierId);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
            cmd.Parameters.AddWithValue("@Status", product.Status);
            cmd.Parameters.AddWithValue("@Description", product.Description);
            cmd.Parameters.AddWithValue("@Image", ImageFileName);

            connection.Open();

            // Insert product table -> return id
            int insertedId = Convert.ToInt32(cmd.ExecuteScalar());


            // Using insertedId 
            cmd.CommandText = productCategoriesCommand;
            cmd.Parameters.AddWithValue("@ProductId", insertedId);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result > 0;
        }

        public bool UpdateProduct(Product product, int categoryId, string ImageFileName)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            string productsCommand = "UPDATE Products SET SupplierId = @SupplierId, NAME = @Name, Price =  @Price, Quantity = @Quantity, Status = @Status, Description = @Description, Image = @Image WHERE Id = @Id;";
            string productCategoriesCommand = "UPDATE ProductCategories SET CategoryId = @CategoryId WHERE ProductId = @Id";

            cmd.CommandText = productsCommand + productCategoriesCommand;
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@Id", product.Id);
            cmd.Parameters.AddWithValue("@SupplierId", product.SupplierId);
            cmd.Parameters.AddWithValue("@Name", product.Name);
            cmd.Parameters.AddWithValue("@Price", product.Price);
            cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
            cmd.Parameters.AddWithValue("@Status", product.Status);
            cmd.Parameters.AddWithValue("@Description", product.Description);
            cmd.Parameters.AddWithValue("@Image", ImageFileName);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);


            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();


            return result > 0;
        }
        
        public bool DeleteProduct(int id)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();
            string productCategoriesCmd = "DELETE FROM ProductCategories WHERE ProductId = @id;";
            string discountProductsCmd = "DELETE FROM DiscountProducts WHERE ProductId = @id;";
            string prouductReviewsCmd = "DELETE FROM ProductReviews WHERE ProductId = @id;";


            cmd.CommandText = productCategoriesCmd + discountProductsCmd + "DELETE FROM Products WHERE Id = @id";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();


            return result > 0;
        }

        public List<DiscountProduct> GetDiscountProducts()
        {
            List<DiscountProduct> discountProductList = new List<DiscountProduct>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM DiscountProducts";
            cmd.Connection = connection;

            connection.Open();
            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                DiscountProduct discountProduct = new DiscountProduct();

                discountProduct.ProductId = Convert.ToInt32(dataReader["ProductId"].ToString());
                discountProduct.DiscountPrice = decimal.Parse(dataReader["DiscountPrice"].ToString());


                discountProductList.Add(discountProduct);
            }

            connection.Close();


            return discountProductList;
        }

        public bool CreateDiscount(int productId, decimal discountPrice)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "INSERT INTO DiscountProducts(ProductId, DiscountPrice) VALUES(@ProductId, @DiscountPrice)";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@DiscountPrice", discountPrice);

            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result > 0;
        }

        public bool DeleteDiscount(int id)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "DELETE FROM DiscountProducts WHERE ProductId = @id;";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();

            return result > 0;
        }
    }
}