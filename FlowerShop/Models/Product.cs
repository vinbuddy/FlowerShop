using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FlowerShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Hãy nhập tên sản phẩm")]
        public string Name { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }


    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

    }

    public class DiscountProduct
    {
        public int ProductId { get; set; }
        public decimal DiscountPrice { get; set; }

    }
}