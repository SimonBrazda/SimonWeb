using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CL3C.Models;
using ExchangeRatesLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using SimonWebMVC.Models;
using SimonWebMVC.Security;
using SimonWebMVC.ViewModels;

namespace SimonWebMVC.Controllers
{
    // [Authorize(Roles = "Super Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAuthorizationService authorizationService;
        private readonly ILogger<AdministrationController> logger;
        private readonly IEmailService emailService;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IAuthorizationService authorizationService, ILogger<AdministrationController> logger, IEmailService emailService)
        {
            this.authorizationService = authorizationService;
            this.logger = logger;
            this.emailService = emailService;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> CreateRole()
        {
            var dummyRole = new IdentityRole();
            var authorizationResult = await authorizationService.AuthorizeAsync(User, dummyRole, AdministrationOperations.CreateRole);

            if (authorizationResult.Succeeded)
            {
                return View();
            }
            
            return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            var role = new IdentityRole(model.RoleName);
            var authorizationResult = await authorizationService.AuthorizeAsync(User, role, AdministrationOperations.CreateRole);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole { Name = model.RoleName };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    var user = await userManager.GetUserAsync(User);
                    logger.LogInformation("User of Id: {Id} and name: {Name} has created role of Id: {RoleId} and name: {RoleName} at {Time}.", user.Id, user.UserName, identityRole.Id, identityRole.Name, DateTime.Now);
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            var dummyRole = new IdentityRole();
            var authorizationResult = await authorizationService.AuthorizeAsync(User, dummyRole, AdministrationOperations.ListRoles);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            var authorizationResult = await authorizationService.AuthorizeAsync(User, role, AdministrationOperations.EditRole);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            ViewBag.Role = role;

            foreach (var user in await userManager.Users.ToListAsync())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            var authorizationResult = await authorizationService.AuthorizeAsync(User, role, AdministrationOperations.EditRole);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }

            role.Name = model.RoleName;
            var result = await roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var user = await userManager.GetUserAsync(User);
                logger.LogInformation("User of Id: {Id} and name: {Name} has created role of Id: {RoleId} and name: {RoleName} at {Time}.", user.Id, user.UserName, role.Id, role.Name, DateTime.Now);
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            var authorizationResult = await authorizationService.AuthorizeAsync(User, role, AdministrationOperations.EditUsersInRole);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.roleId = roleId;

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in await userManager.Users.ToListAsync())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            var authorizationResult = await authorizationService.AuthorizeAsync(User, role, AdministrationOperations.EditRole);
            var currentUser = await userManager.GetUserAsync(User);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                    logger.LogInformation("User of Id: {Id} and name: {Name} has added user of Id: {UserId} and name: {UserName} to role of Id: {RoleId} and name: {RoleName} at {Time}.", currentUser.Id, currentUser.UserName, user.Id, user.UserName, role.Id, role.Name, DateTime.Now);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        public async Task<IActionResult> ListUsers()
        {
            var dummyUser = new ApplicationUser();
            var authorizationResult = await authorizationService.AuthorizeAsync(User, dummyUser, AdministrationOperations.ListUsers);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            var authorizationResult = await authorizationService.AuthorizeAsync(User, user, AdministrationOperations.EditUser);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                // Throws Requested value was not found exception. The Exception is of type System.ArgumentException. Add catch try block!!!
                DefaultCurrency = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), user.DefaultCurrency),
                Claims = userClaims.Select(c => c.Type + ": " + c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            var authorizationResult = await authorizationService.AuthorizeAsync(User, user, AdministrationOperations.ListUsers);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.DefaultCurrency = model.DefaultCurrency.ToString();

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (user.Email != model.Email)
                    {
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                        await emailService.SendAsync(user.Email, "Email confirmation", $"Click on this link to confirm your email adress:\n {confirmationLink}", false);

                        user.EmailConfirmed = false;
                        await userManager.UpdateAsync(user);
                    }
                    var currentUser = await userManager.GetUserAsync(User);
                    logger.LogInformation("User of Id: {Id} and name: {Name} has edited user of Id: {UserId} and name: {UserName} at {Time}.", currentUser.Id, currentUser.UserName, user.Id, user.UserName, DateTime.Now);
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            var authorizationResult = await authorizationService.AuthorizeAsync(User, user, AdministrationOperations.EditUser);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            if(userManager.SetLockoutEnabledAsync(user, true).Result.Succeeded &&
            userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(1000)).Result.Succeeded)
            {
                logger.LogInformation("User of Id: {Id} and name: {Name} has been locked at {Time}.", user.Id, user.UserName, DateTime.Now);
                ViewBag.Title = "Success";
                ViewBag.Message = "Account has been successfully locked.";
                return View("Confirmation");
            }

            ViewBag.ErrorTitle = "Fail";
            ViewBag.ErrorMessage = "Failed to lock the account. Plese review logs to see what went wrong.";
            return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            var authorizationResult = await authorizationService.AuthorizeAsync(User, user, AdministrationOperations.EditUser);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            if(userManager.SetLockoutEnabledAsync(user, true).Result.Succeeded &&
            userManager.SetLockoutEndDateAsync(user, null).Result.Succeeded)
            {
                logger.LogInformation("User of Id: {Id} and name: {Name} has been unlocked at {Time}.", user.Id, user.UserName, DateTime.Now);
                ViewBag.Title = "Success";
                ViewBag.Message = "Account has been successfully unlocked.";
                return View("Confirmation");
            }

            ViewBag.ErrorTitle = "Fail";
            ViewBag.ErrorMessage = "Failed to unlock the account. Plese review logs to see what went wrong.";
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            var authorizationResult = await authorizationService.AuthorizeAsync(User, user, AdministrationOperations.ListUsers);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                logger.LogInformation("User of Id: {Id} and name: {Name} has been deleted at {Time}.", user.Id, user.UserName, DateTime.Now);
                return RedirectToAction(nameof(ListUsers));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(nameof(ListUsers));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            try
            {
                var result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    logger.LogInformation("Role of Id: {Id} and name: {Name} has been deleted at {Time}.", role.Id, role.Name, DateTime.Now);
                    return RedirectToAction(nameof(ListRoles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(nameof(ListRoles));
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error deleting role {ex}");

                ViewBag.ErrorTitle = $"{role.Name} role is in use";
                ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. If you want to delete this role, please remove the users from the role and then try to delete it again";
                return View("Error");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(userId);

            var authorizationResult = await authorizationService.AuthorizeAsync(User, user, AdministrationOperations.ListUsers);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in roleManager.Roles.ToList())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = await userManager.IsInRoleAsync(user, role.Name)
                };

                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            var authorizationResult = await authorizationService.AuthorizeAsync(User, user, AdministrationOperations.ListUsers);

            if (authorizationResult.Succeeded == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (result.Succeeded == false)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, model.Where(r => r.IsSelected).Select(r => r.RoleName));

            if (result.Succeeded == false)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
            }

            return RedirectToAction(nameof(EditUser), new { Id = userId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            foreach (var claim in ClaimsStore.AllClaims)
            {
                var userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (result.Succeeded == false)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            result = await userManager.AddClaimsAsync(user, model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (result.Succeeded == false)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction(nameof(EditUser), new { Id = model.UserId });
        }
    }
}