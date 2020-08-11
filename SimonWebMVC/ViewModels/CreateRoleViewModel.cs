using System.ComponentModel.DataAnnotations;

namespace SimonWebMVC.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}