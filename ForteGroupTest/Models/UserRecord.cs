using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ForteGroupTest.Models
{
    public class UserRecord
    {
        public int AuthorId { get; set; }
        [Required(ErrorMessage = "You haven't entered any note", AllowEmptyStrings=false)]
        public string Content { get; set; }
        public System.DateTime Date { get; set; }
        public int isRemoved { get; set; }
    }
}