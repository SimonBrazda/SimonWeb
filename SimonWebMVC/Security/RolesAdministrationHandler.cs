using System.Threading.Tasks;
using CL3C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SimonWebMVC.Security
{
    public class RolesAdministrationHandler : AuthorizationHandler<OperationRequirement, IdentityRole>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public RolesAdministrationHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationRequirement requirement, IdentityRole resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.ListRoles &&
                requirement.Name != Constants.CreateRole &&
                requirement.Name != Constants.EditRole &&
                requirement.Name != Constants.EditRoleName &&
                requirement.Name != Constants.EditUsersInRole &&
                requirement.Name != Constants.DeleteRole )
            {
                return Task.CompletedTask;
            }

            if(context.User.Identity.IsAuthenticated == false)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Constants.SuperAdminRole))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(Constants.AdminRole))
            {
                if (requirement.Name == AdministrationOperations.ListRoles.Name)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                if (resource.Name == Constants.UserRole)
                {
                    if (requirement.Name == Constants.EditRole || requirement.Name == Constants.EditUsersInRole)
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }
                
            }

            return Task.CompletedTask;
        }
    }
}