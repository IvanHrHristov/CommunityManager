using CommunityManager.Infrastructure.Data.Models;
using CommunityManager.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.RoleConstants;
using CommunityManager.Core.Models.User;
using System.Drawing;

namespace CommunityManager.Areas.Administration.Controllers
{
    [Authorize(Roles = Administrator)]
    [Area(AdminArea)]
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

        public async Task<IActionResult> AddUserToRoll(string role, string id, Guid communityId)
        {
            var user = await userManager.FindByIdAsync(id);

            if (role == "Supervisor")
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return RedirectToAction("Open", "Community", new { id = communityId });
        }
    }
}
