using CommunityManager.Infrastructure.Data.Models;
using CommunityManager.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.RoleConstants;
using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.User;

namespace CommunityManager.Controllers
{
    /// <summary>
    /// Controller to manage users
    /// </summary>
    [Authorize]
    public class UserController : Controller
    {
        /// <summary>
        /// Providing access to the UserManager 
        /// </summary>
        private readonly UserManager<ApplicationUser> userManager;
        /// <summary>
        /// Providing access to the SignInManager 
        /// </summary>
        private readonly SignInManager<ApplicationUser> signInManager;
        /// <summary>
        /// Providing access to the RoleManager 
        /// </summary>
        private readonly RoleManager<IdentityRole> roleManager;
        /// <summary>
        /// Service providing methods to manage users
        /// </summary>
        private readonly IUserServices userServices;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUserServices userServices)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.userServices = userServices;
        }

        /// <summary>
        /// Redirects the user to the register view 
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterViewModel();

            return View(model);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="model">Register view model</param>
        /// <returns>Redirects the user to the login view</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await userManager.CreateAsync(userServices.CreateUserAsync(model), model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "User");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        /// <summary>
        /// Redirects the user to the login view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel();

            return View(model);
        }

        /// <summary>
        /// Logs in the user
        /// </summary>
        /// <param name="model">Login view model</param>
        /// <returns>Redirects the user to the home view</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userServices.GetUserAsync(model.UserName);

            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login.");

            return View(model);
        }

        /// <summary>
        /// Logs out the user 
        /// </summary>
        /// <returns>Redirects the user to the home view</returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Redirects the user to the edit view
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(User);

            var model = new EditViewModel()
            {
                UserName = user.UserName,
                Email = user.Email,
                Age = user.Age
            };

            ViewBag.Id = user.Id;

            return View(model);
        }

        /// <summary>
        /// Edits the details of a users account
        /// </summary>
        /// <param name="model">Edit view model</param>
        /// <returns>Redirects the user to the home view</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userServices.EditUserAsync(model);

            await userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Redirects the user to the change passowrd view
        /// </summary>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel();

            return View(model);
        }

        /// <summary>
        /// Changes the password of the current user
        /// </summary>
        /// <param name="model">Change password view model</param>
        /// <returns>Redirects the user to the home view</returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.GetUserAsync(User);

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
            
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        /// <summary>
        /// Creates the Administrator and Supervisor roles
        /// </summary>
        /// <returns>Redirects the user to the home view</returns>
        public async Task<IActionResult> CreateRoles()
        {
            await roleManager.CreateAsync(new IdentityRole(Supervisor));
            await roleManager.CreateAsync(new IdentityRole(Administrator));

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Development only method to add an Administrator role to test account
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AddAdminToRole()
        {
            var user = await userManager.FindByNameAsync("Admin");

            await userManager.AddToRoleAsync(user, Administrator);

            return RedirectToAction("Index", "Home");
        }
    }
}
