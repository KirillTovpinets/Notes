using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ForteGroupTest.Models
{
    public class LogedUser
    {
        [Required(ErrorMessage = "Enter login", AllowEmptyStrings = false)]
        public string Login { get; set; }

        [Required(ErrorMessage = "Enter password", AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}