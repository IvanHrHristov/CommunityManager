using CommunityManager.Infrastructure.Data.Models;
using CommunityManager.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.RoleConstants;

namespace CommunityManager.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.UserName,
                CreatedOn = DateTime.UtcNow,
                Age = model.Age
            };

            var result = await userManager.CreateAsync(user, model.Password);

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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.UserName);

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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CreateRoles()
        {
            await roleManager.CreateAsync(new IdentityRole(Supervisor));
            await roleManager.CreateAsync(new IdentityRole(Administrator));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> AddAdminToRole()
        {
            var user = await userManager.FindByNameAsync("Admin");

            await userManager.AddToRoleAsync(user, Administrator);

            return RedirectToAction("Index", "Home");
        }
    }
}
