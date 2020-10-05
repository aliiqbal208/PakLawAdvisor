using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PakLawAdvisor.Models
{
    public class SignUp
    {
        public int Usr_id { get; set; }
        [Display(Name = "First Name", Description = "First Name ")]
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
            "Numbers and special characters are not allowed in the name.")]
        public string First_Name { get; set; }
        [Display(Name = "Last Name", Description = "Last Name ")]
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage =
            "Numbers and special characters are not allowed in the name.")]
        public string Second_Name { get; set; }
        public string Email { get; set; }
      
        public string Password { get; set; }
        [Display(Name = "Confirm new password")]
        [Required(ErrorMessage = "Enter Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string Domain { get; set; }
        public string Status { get; set; }
        public string IS_ACTIVE { get; set; }



    }
}