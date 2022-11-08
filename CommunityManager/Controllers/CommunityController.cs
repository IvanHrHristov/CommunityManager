using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Community;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.MessageConstants;

namespace CommunityManager.Controllers
{
    public class CommunityController : Controller
    {
        private readonly IRepository repository;
        private readonly ICommunityServices communityService;
        private readonly UserManager<ApplicationUser> userManager;

        public CommunityController(
            IRepository repository,
            ICommunityServices communityService,
            UserManager<ApplicationUser> userManager)
        {
            this.repository = repository;
            this.communityService = communityService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> All()
        {
            var currentUserId = User.Id();

            var model = await communityService.GetAllAsync();

            ViewBag.Title = "All Communities";
            ViewBag.currentUserId = currentUserId;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Open(Guid id)
        {
            var model = await communityService.GetCommunityByIdAsync(id);

            if (model == null)
            {
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction(nameof(All));
            }

            return View(model);
        }
    }
}
