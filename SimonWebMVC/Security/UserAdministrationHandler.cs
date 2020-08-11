using System.Threading.Tasks;
using CL3C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SimonWebMVC.Security
{
    public class UserAdministrationHandler : AuthorizationHandler<OperationRequirement, ApplicationUser>
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public UserAdministrationHandler(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;

        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationRequirement requirement, ApplicationUser resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.CreateUser &&
                requirement.Name != Constants.EditUser &&
                requirement.Name != Constants.DeleteUser &&
                requirement.Name != Constants.ListUsers &&
                requirement.Name != Constants.ManageUserRoles)
            {
                return Task.CompletedTask;
            }

            // SuperAdmin can do anything
            if (context.User.IsInRole(Constants.SuperAdminRole))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Admin can edit regular user
            if (context.User.IsInRole(Constants.AdminRole) &&
            (userManager.IsInRoleAsync(resource, Constants.AdminRole).Result == false &&
            (userManager.IsInRoleAsync(resource, Constants.SuperAdminRole).Result == false)))
            {
                
                context.Succeed(requirement);    

                
                return Task.CompletedTask;
            }

            // User can edit only himself
            if (context.User.IsInRole(Constants.UserRole) &&
            userManager.GetUserId(context.User) == resource.Id)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}