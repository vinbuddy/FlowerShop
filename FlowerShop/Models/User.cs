using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FlowerShop.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username cant be blank")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password cant be blank")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password cant be blank")]
        [Compare("Password", ErrorMessage = "Confirm password do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email cant be blank")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        public string Avatar { get; set; }
    }
}