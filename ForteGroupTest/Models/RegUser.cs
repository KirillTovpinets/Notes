using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ForteGroupTest.Models
{
    public class RegUser
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Enter your login", AllowEmptyStrings = false)]
        public string Login { get; set; }
        [Required(ErrorMessage = "Enter your password", AllowEmptyStrings = false)]
        public string AdminKey { get; set; }
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords are not equal")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Enter the number from the captha", AllowEmptyStrings = false)]
        public string Captcha { get; set; }
        [Required(ErrorMessage = "Enter your name", AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter your sirname", AllowEmptyStrings = false)]
        public string Sirname { get; set; }
        [Required(ErrorMessage = "Enter your patername", AllowEmptyStrings = false)]
        public string Patername { get; set; }
        public int RoleId { get; set; }
    }
}