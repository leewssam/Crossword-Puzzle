using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SEF_Assignment.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SEF_Assignment.ViewModel
{
    public class RegisterStu
    {
        [Display(Name = "UserID")]
        [Required(ErrorMessage = "User ID is required.")]
        public string Stu_ID { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Stu_Pass { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please enter your password again.")]
        public string Stu_Pass_Confirm { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        public string Stu_Name { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Stu_Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        public string Stu_PhoneNo { get; set; }

        [Display(Name = "Class ID")]
        [Required(ErrorMessage = "Class ID is required.")]
        public string Class_ID { get; set; }
    }
}