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
    /// <summary>
    /// Controller to manage users
    /// </summary>
    [Authorize(Roles = Administrator)]
    [Area(AdminArea)]
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
        /// Adds a super to a role
        /// </summary>
        /// <param name="role">Role to add to user</param>
        /// <param name="id">ID of the user</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Redirects the user </returns>
        public async Task<IActionResult> AddUserToRoll(string role, string id, Guid communityId)
        {
            var user = await userManager.FindByIdAsync(id);

            if (role == "Supervisor")
            {
                await userManager.AddToRoleAsync(user, role);
            }

            if (role == "User" && await userManager.IsInRoleAsync(user, "Supervisor"))
            {
                await userManager.RemoveFromRoleAsync(user, "Supervisor");
            }

            return RedirectToAction("Open", "Community", new { id = communityId });
        }
    }
}
