using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SEF_Assignment.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SEF_Assignment.ViewModel
{
    public class RegisterClass
    {
        [Display(Name = "Class ID")]
        public string Class_ID { get; set; }

        [Display(Name = "Class Name")]
        [Required(ErrorMessage = "Class Name is required.")]
        public string Class_Name { get; set; }


    }
}