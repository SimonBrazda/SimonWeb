using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExchangeRatesLib;
using Microsoft.AspNetCore.Mvc;
using SimonWebMVC.Extensions;

namespace SimonWebMVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Default Currency")]
        public CurrencyEnum DefaultCurrency { get; set; }

        // [Required]
        [Display(Name = "I agree with terms of Privacy Policy which are listed")]
        [MustBeTrue(ErrorMessage = "Must be ticked!")]
        // [Range(typeof(bool), "true", "true", ErrorMessage="The field Privacy policy must be checked.")]
        public bool PrivacyPolicy { get; set; }
    }
}