using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleInsuranceManagement.Models
{
    public class ChangePassword
    {
        [Display(Name = "Old Password")]
        [Required(ErrorMessage = "Old Password is required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"[A-Za-z0-9!@#$%^&*_]{8,}", ErrorMessage = "Password must contain at least 8 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "New Password and Confirm Password are not the same")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}