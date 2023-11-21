using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace FlowerShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal ShippingCost { get; set; }

        public string OrderDate { get; set; }
        public string ReceiveDate { get; set; }
        public string ReceiveTime { get; set; }
        public string Note { get; set; }
    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public decimal OrderPrice { get; set; }

        public decimal TotalOrderPrice { get; set; }

    }
    public class ShippingOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Nhập số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Nhập địa chỉ giao hàng")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Chọn tỉnh thành")]
        public string City { get; set; }

        [Required(ErrorMessage = "Chọn quận huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Chọn phường xã")]
        public string Ward { get; set; }

        [Required(ErrorMessage = "Nhập họ tên đầy đủ")]
        public string FullName { get; set; }

    }

    public class OrderStatus
    {
        public string StatusName { get; set; }
        public int StatusQuantity { get; set; }
    }

}