using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SEF_Assignment.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SEF_Assignment.ViewModel
{
    public class Login
    {
        [Required]
        public string UserID { get; set; }
        [Required]
        public string UserPassword { get; set; }

    }
}