using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FlowerShop.Models;
using FlowerShop.DBContext;


namespace FlowerShop.ApiControllers
{
    public class CartController : ApiController
    {
        public List<CartItem> GetCartItems(int userId)
        {
            CartDB cartDB = new CartDB();
            List<CartItem> cartItems = cartDB.GetCartItems(userId);

            return cartItems;
        }
        public List<CartItem> Post(int userId, int productId, int quantity)
        {
            CartDB cartDB = new CartDB();
            bool isSuccess = cartDB.AddItem(userId, productId, quantity);

            if (!isSuccess)
            {
                return new List<CartItem>();
            }

            return cartDB.GetCartItems(userId);
        }

        public List<CartItem> Put(int userId, int productId, int quantity)
        {
            CartDB cartDB = new CartDB();
            bool isSuccess = cartDB.UpdateQuantity(userId, productId, quantity);

            return cartDB.GetCartItems(userId);
        }

        public bool PutIncreaseQuantity(int userId, int productId)
        {
            CartDB cartDB = new CartDB();
            bool isSuccess = cartDB.UpdateIncreaseQuantity(userId, productId);

            return isSuccess;
        }
        public bool PutDecreaseQuantity(int userId, int productId)
        {
            CartDB cartDB = new CartDB();
            bool isSuccess = cartDB.UpdateDecreaseQuantity(userId, productId);

            return isSuccess;
        }

        public bool Delete(int userId, int productId)
        {
            CartDB cartDB = new CartDB();
            bool isSuccess = cartDB.DeleteCartItem(userId, productId);

            return isSuccess;
        }

    }
}
