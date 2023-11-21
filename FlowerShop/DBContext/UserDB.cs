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
    public class UserDB
    {
        public string connectStr = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public List<User> GetUsers()
        {
            List<User> userList = new List<User>();

            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM Users";
            cmd.Connection = connection;

            connection.Open();

            SqlDataReader dataReader = cmd.ExecuteReader();
            var ok = dataReader.HasRows;

            // Get rows in table
            while (dataReader.Read())
            {
                User user = new User();

                user.Id = Convert.ToInt32(dataReader["Id"].ToString());
                user.UserName = dataReader["UserName"].ToString();
                user.Email = dataReader["Email"].ToString();
                user.Password = dataReader["Password"].ToString();
                user.Avatar = dataReader["Avatar"].ToString();


                userList.Add(user);
            }

            connection.Close();


            return userList;
        }

        public Tuple<bool, int> CreateUser(RegisterVM registerInfo)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "INSERT INTO Users(Email, Password, UserName, Avatar) VALUES (@email, @password, @username, @avatar); SELECT SCOPE_IDENTITY();";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@email", registerInfo.Email);
            cmd.Parameters.AddWithValue("@password", registerInfo.Password);
            cmd.Parameters.AddWithValue("@username", registerInfo.UserName);
            cmd.Parameters.AddWithValue("@avatar", "DefaultUserAvatar.jpg");


            connection.Open();

            // int result = cmd.ExecuteNonQuery();

            int newID = Convert.ToInt32(cmd.ExecuteScalar());
           

            connection.Close();

            // Create successfully
            if (newID > 0)
            {
                return Tuple.Create(true, newID);
                //return true;
            }

            //return false;
            return Tuple.Create(false, newID);

        }

        public bool UpdateInfo(User userInfo, string ImageFileName)
        {
            SqlConnection connection = new SqlConnection(connectStr);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "UPDATE Users SET UserName =  @UserName, Avatar = @Avatar WHERE Id = @Id;";
            cmd.Connection = connection;

            cmd.Parameters.AddWithValue("@Id", userInfo.Id);
            cmd.Parameters.AddWithValue("@Avatar", ImageFileName);
            cmd.Parameters.AddWithValue("@UserName", userInfo.UserName);


            connection.Open();

            int result = cmd.ExecuteNonQuery();

            connection.Close();


            return result > 0;
        }

       
        public bool isExisted(string userName, string email)
        {
            List<User> users = GetUsers();
            bool result = users.Any(user => user.UserName == userName || user.Email == email);
            return result;
        }
    }
}