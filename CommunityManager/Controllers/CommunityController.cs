using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.MessageConstants;

namespace CommunityManager.Controllers
{
    public class CommunityController : Controller
    {
        private readonly IRepository repository;
        private readonly ICommunityServices communityService;

        public CommunityController(
            IRepository repository,
            ICommunityServices communityService)
        {
            this.repository = repository;
            this.communityService = communityService;
        }

        public async Task<IActionResult> All(string? errorMessage)
        {
            var currentUserId = User.Id();

            var model = await communityService.GetAllAsync();

            ViewBag.currentUserId = currentUserId;
            ViewBag.errorMessage = errorMessage;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Join(Guid id)
        {
            var currentUserId = User.Id();

            var community = await repository.GetByIdAsync<Community>(id);
            var user = await repository.GetByIdAsync<ApplicationUser>(currentUserId);

            if (community == null)
            {
                return NotFound();
            }

            if (user == null)
            {
                return NotFound();
            }

            if (community.AgeRestricted)
            {
                if (user.Age < 18)
                {
                    var errorMessage = "You are too young to join that community";

                    return RedirectToAction(nameof(All), new { errorMessage = errorMessage });
                }
            }

            await communityService.JoinCommunityAsync(id, currentUserId);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Mine(string? errorMessage)
        {
            var currentUserId = User.Id();

            var model = await communityService.GetMineAsync(currentUserId);

            ViewBag.currentUserId = currentUserId;
            ViewBag.errorMessage = errorMessage;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Open(Guid id, string? manageErrorMessage)
        {
            var currentUserId = User.Id();

            ViewBag.currentUserId = currentUserId;
            ViewBag.errorMessage = manageErrorMessage;

            var model = await communityService.GetCommunityByIdAsync(id);

            if (model.Name == null)
            {
                var errorMessage = "The community you are trying to open does not exist";

                return RedirectToAction(nameof(Mine), new { errorMessage = errorMessage });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AddMarketplace(Guid communityId)
        {
            var model = new AddMarketplaceViewModel();

            ViewBag.CommunityId = communityId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMarketplace(AddMarketplaceViewModel model, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await communityService.AddMarketplaceToCommunityAsync(model, id);

            return RedirectToAction(nameof(Open), new { id });
        }

        [HttpGet]
        public IActionResult AddChatroom(Guid communityId)
        {
            var model = new AddChatroomViewModel();

            ViewBag.CommunityId = communityId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddChatroom(AddChatroomViewModel model, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var creatorId = User.Id();

            await communityService.AddChatroomToCommunityAsync(model, id, creatorId);

            return RedirectToAction(nameof(Open), new { id });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var creatorId = User.Id();

            ViewBag.CreatorId = creatorId;

            var model = new CreateCommunityViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommunityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await communityService.CreateCommunityAsync(model);

            return RedirectToAction(nameof(Mine));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, string? manageErrorMessage)
        {
            var currentUserId = User.Id();

            ViewBag.currentUserId = currentUserId;
            ViewBag.errorMessage = manageErrorMessage;

            var model = await communityService.GetCommunityByIdAsync(id);

            if (model.Name == null)
            {
                var errorMessage = "The community you are trying to open does not exist";

                return RedirectToAction(nameof(Mine), new {errorMessage = errorMessage});
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(Guid id)
        {
            var community = await communityService.GetCommunityByIdAsync(id);

            if (community.Name == null)
            {
                var errorMessage = "The community you are trying to open does not exist";

                return RedirectToAction(nameof(Mine), new { errorMessage = errorMessage });
            }

            CreateCommunityViewModel model = new CreateCommunityViewModel()
            {
                Name = community.Name,
                Description = community.Description,
                CreatedOn = community.CreatedOn,
                AgeRestricted = community.AgeRestricted,
                CreatorId = community.CreatorId
            };

            ViewBag.CreatorId = community.CreatorId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(Guid id, CreateCommunityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await communityService.ManageCommunityAsync(id, model);

            return RedirectToAction(nameof(Mine));
        }

        public async Task<IActionResult> Leave(Guid id)
        {
            var userId = User.Id();

            try
            {
                await communityService.LeaveCommunityAsync(id, userId);

                return RedirectToAction(nameof(Mine));
            }
            catch (Exception)
            {
                var errorMessage = "The community you are trying to leave does not exist";

                return RedirectToAction(nameof(Mine), new { errorMessage = errorMessage });
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await communityService.DeleteCommunityAsync(id);

                return RedirectToAction(nameof(Mine));
            }
            catch (Exception)
            {
                var errorMessage = "The community you are trying to delete does not exist";

                return RedirectToAction(nameof(Mine), new { errorMessage = errorMessage });
            }
        }

        public async Task<IActionResult> DeleteMarketplace(Guid id, Guid communityId)
        {
            try
            {
                await communityService.DeleteMarketplaceAsync(id);

                return RedirectToAction(nameof(Open), new { id = communityId});
            }
            catch (Exception)
            {
                var errorMessage = "The marketplace you are trying to delete does not exist";

                return RedirectToAction(nameof(Open), new { id = communityId, manageErrorMessage = errorMessage });
            }
        }

        public async Task<IActionResult> DeleteChatroom(Guid id, Guid communityId)
        {
            try
            {
                await communityService.DeleteChatroomAsync(id);

                return RedirectToAction(nameof(Open), new { id = communityId});
            }
            catch (Exception)
            {
                var errorMessage = "The chatroom you are trying to delete does not exist";

                return RedirectToAction(nameof(Open), new { id = communityId, manageErrorMessage = errorMessage });
            }
            
        }
    }
}
