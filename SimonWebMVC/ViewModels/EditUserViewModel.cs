using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExchangeRatesLib;

namespace SimonWebMVC.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(3)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Default Currency")]
        public CurrencyEnum DefaultCurrency { get; set; }

        public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }

        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
    }
}