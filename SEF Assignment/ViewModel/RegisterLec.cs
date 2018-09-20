using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SEF_Assignment.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SEF_Assignment.ViewModel
{
    public class RegisterLec
    {
        [Display(Name = "UserID")]
        [Required(ErrorMessage = "User ID is required.")]
        public string Lec_ID { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Lec_Pass { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please enter your password again.")]
        public string Lec_Pass_Confirm { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        public string Lec_Name { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Lec_Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        public string Lec_PhoneNo { get; set; }
    }
}