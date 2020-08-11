using Microsoft.AspNetCore.Authorization;

namespace SimonWebMVC.Security
{
    public class OperationRequirement : IAuthorizationRequirement
    {
        public string Name { get; set; }
    }
}