using System.ComponentModel.DataAnnotations;

namespace SimonWebMVC.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}