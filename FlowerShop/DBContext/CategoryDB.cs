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
    public class CategoryDB
    {
        public string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
        public List<Category> GetCategories()
        {
            List<Category> categoryList = new List<Category>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM Categories";
            cmd.Connection = connection;

            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                Category category = new Category();

                category.Id = Convert.ToInt32(dataReader["Id"].ToString());
                category.CategoryName = dataReader["CategoryName"].ToString();


                categoryList.Add(category);
            }

            connection.Close();


            return categoryList;
        }

        public List<ProductCategory> GetProductCategories()
        {
            List<ProductCategory> productCategoryList = new List<ProductCategory>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM ProductCategories";
            cmd.Connection = connection;

            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();

            // Get rows in table
            while (dataReader.Read())
            {
                ProductCategory productCategory = new ProductCategory();

                productCategory.ProductId = Convert.ToInt32(dataReader["ProductId"].ToString());
                productCategory.CategoryId = Convert.ToInt32(dataReader["CategoryId"].ToString());


                productCategoryList.Add(productCategory);
            }

            connection.Close();


            return productCategoryList;
        }
    }
}