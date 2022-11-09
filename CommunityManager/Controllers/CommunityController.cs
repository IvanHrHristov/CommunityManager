using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Services;
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

            ViewBag.currentUserId = currentUserId;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Join(Guid communityId)
        {
            var currentUserId = User.Id();

            await communityService.JoinCommunityAsync(communityId, currentUserId);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Mine()
        {
            var currentUserId = User.Id();

            var model = await communityService.GetMineAsync(currentUserId);

            ViewBag.currentUserId = currentUserId;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Open(Guid id)
        {
            var currentUserId = User.Id();

            ViewBag.currentUserId = currentUserId;

            var model = await communityService.GetCommunityByIdAsync(id);

            if (model == null)
            {
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction(nameof(All));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Add(Guid communityId)
        {
            var model = new AddMarketplaceViewModel();

            ViewBag.CommunityId = communityId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddMarketplaceViewModel model, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await communityService.AddMarketplaceToCommunityAsync(model, id);

                return RedirectToAction(nameof(Open), new { id });
            }
            catch (Exception)
            {

                ModelState.AddModelError("", "Something went wrong.");

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var creatorId = User.Id();

            ViewBag.CreatorId = creatorId;

            var model = new CreateCommunityViewModel();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommunityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await communityService.CreateCommunityAsync(model);

                return RedirectToAction(nameof(All));
            }
            catch (Exception)
            {

                ModelState.AddModelError("", "Something went wrong.");

                return View(model);
            }
        }
    }
}
