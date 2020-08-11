using System.Collections.Generic;
using System.Security.Claims;

namespace SimonWebMVC.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Delete Role", "Delete Role"),
            new Claim("Edit Role", "Edit Role")
        };
    }
}