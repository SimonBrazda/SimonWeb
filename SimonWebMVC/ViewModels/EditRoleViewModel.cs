using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimonWebMVC.ViewModels
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        [MinLength(3)]
        [MaxLength(20)]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }

        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
    }
}