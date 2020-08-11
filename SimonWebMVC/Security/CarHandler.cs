using System.Threading.Tasks;
using CL3C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SimonWebMVC.Security
{
    public class CarHandler : AuthorizationHandler<OperationRequirement, BaseCar>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public CarHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationRequirement requirement, BaseCar resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name != Constants.CreateCar &&
                requirement.Name != Constants.EditCar &&
                requirement.Name != Constants.DeleteCar )
            {
                return Task.CompletedTask;
            }
            else if(context.User.Identity.IsAuthenticated == false)
            {
                return Task.CompletedTask;
            }

            // Any of the App users can create cars
            if (requirement.Name == Constants.CreateCar && (context.User.IsInRole(Constants.UserRole) || context.User.IsInRole(Constants.AdminRole) || context.User.IsInRole(Constants.SuperAdminRole)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // SuperAdmin can edit any car
            if (context.User.IsInRole(Constants.SuperAdminRole))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var ownerUser = (userManager.FindByNameAsync(resource.Owner)).Result;

            // Admin can edit any car except SuperAdmins's
            if (context.User.IsInRole(Constants.AdminRole) && (userManager.IsInRoleAsync(ownerUser, Constants.SuperAdminRole).Result) == false)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // User can edit only his cars
            if (resource.Owner.ToLower() == userManager.GetUserAsync(context.User).Result.UserName.ToLower())
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}