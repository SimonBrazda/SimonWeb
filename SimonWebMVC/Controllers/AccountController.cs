using System.Threading.Tasks;
using CL3C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimonWebMVC.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using SimonWebMVC.Security;
using System;
using NETCore.MailKit.Core;
using reCAPTCHA.AspNetCore.Attributes;
using SimonWebMVC.Extensions;
using ExchangeRatesLib;

namespace SimonWebMVC.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IEmailService emailService;
        private readonly IDataProtector protector;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 ILogger<AccountController> logger,
                                 IEmailService emailService,
                                 IDataProtectionProvider dataProtectionProvider,
                                 DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            this.logger = logger;
            this.emailService = emailService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.UserIdRouteValue);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateRecaptcha]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, DefaultCurrency = model.DefaultCurrency.ToString() };
                var result = await userManager.CreateAsync(user, model.Password);
                await userManager.AddToRoleAsync(user, Constants.UserRole);

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                    await emailService.SendAsync(user.Email, "Email confirmation", $"Click on this link to confirm your email adress:\n {confirmationLink}", false);

                    logger.LogInformation("New user of Id: {Id} and name: {Name} has been created at {Time}.", user.Id, user.UserName, DateTime.Now);

                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    // await signInManager.SignInAsync(user, isPersistent: false);
                    // return RedirectToAction("Index", "Home");

                    ViewBag.Title = "Registration successful";
                    ViewBag.Message = "Before you can login, please confirm your email, by clicking on the confirmation link we have emailed you.";
                    return View("Successful");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(User);

            EditAccountViewModel model = new EditAccountViewModel()
            {
                DefaultCurrency = user.DefaultCurrency.ToEnum<CurrencyEnum>()
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAccountViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            user.DefaultCurrency = model.DefaultCurrency.ToString();
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                logger.LogInformation("User of Id: {Id} and name: {Name} has been edited at {Time}.", user.Id, user.UserName, DateTime.Now);
                ViewBag.Title = "Account Edit Result";
                ViewBag.Message = "Your account has been succesfully edited.";
                return View("Successful");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Delete()
        {
            ViewBag.Title = "Do you wish to delete your account?";
            ViewBag.Message = "Your account can be retrieved after deletion. If you wish to get your account back please contact support.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LockAccount()
        {
            var user = await userManager.GetUserAsync(User);
            var result = await userManager.SetLockoutEnabledAsync(user, true);
            await userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(1000));

            if (result.Succeeded)
            {
                logger.LogInformation("User of Id: {Id} and name: {Name} has been locked at {Time}.", user.Id, user.UserName, DateTime.Now);
                ViewBag.Title = "Account Deleted";
                ViewBag.Message = "Your account has been succesfully deleted.";
                await signInManager.SignOutAsync();
                return View("Successful");
            }

            ViewBag.Title = "Your account could not be deleted.";
            ViewBag.Message = "Please contact support for more info.";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }

            return Json($"Email {email} is already taken");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed.";
            return View("Error");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && user.EmailConfirmed == false &&
                    await userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Email has not been confirmed yet.");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(userManager.FindByEmailAsync(model.Email).Result.UserName, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(returnUrl) == false && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    logger.LogInformation("User of Id: {Id} and name: {Name} has loged in at {Time}.", user.Id, user.UserName, DateTime.Now);

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut == true)
                {
                    return View("AccountLocked");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                    await emailService.SendAsync(user.Email, "Password reset", $"Click on this link to reset your password:\n {passwordResetLink}", false);

                    logger.LogInformation("User of Id: {Id} and name: {Name} has requested password reset link at {Time}.", user.Id, user.UserName, DateTime.Now);

                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token.");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        logger.LogInformation("User of Id: {Id} and name: {Name} has successfully reseted his password at {Time}.", user.Id, user.UserName, DateTime.Now);
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);
                logger.LogInformation("User of Id: {Id} and name: {Name} has changed his password at {Time}.", user.Id, user.UserName, DateTime.Now);
                return View("PasswordChangeConfirmation");
            }
            
            return View(model);
        } 

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}