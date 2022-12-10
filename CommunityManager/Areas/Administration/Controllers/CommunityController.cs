using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.RoleConstants;

namespace CommunityManager.Areas.Administration.Controllers
{
    [Authorize(Roles = Administrator)]
    [Area(AdminArea)]
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
                
        public async Task<IActionResult> Mine()
        {
            var currentUserId = User.Id();

            var model = await communityService.GetMineForAdminAsync(currentUserId);

            ViewBag.currentUserId = currentUserId;

            var stringArray = new string[model.Count()];
            int i = 0;

            foreach (var community in model)
            {
                stringArray[i] = "data:image/png;base64," + Convert.ToBase64String(community.Photo, 0, community.PhotoLenght);
                i++;
            }

            ViewBag.Base64StringCollection = stringArray;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Open(Guid id)
        {
            var currentUserId = User.Id();

            ViewBag.currentUserId = currentUserId;

            var model = await communityService.GetCommunityByIdForAdminAsync(id);

            if (!await communityService.CheckCommunityMemberId(id, currentUserId))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (model.Name == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AddMarketplace()
        {
            var model = new AddMarketplaceViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMarketplace(AddMarketplaceViewModel model, Guid id)
        {
            if (!await communityService.CheckCommunityCreatorId(id, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await communityService.AddMarketplaceToCommunityAsync(model, id);

            return RedirectToAction(nameof(Open), new { id });
        }

        [HttpGet]
        public IActionResult AddChatroom()
        {
            var model = new AddChatroomViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddChatroom(AddChatroomViewModel model, Guid id)
        {
            if (!await communityService.CheckCommunityCreatorId(id, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

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
        public async Task<IActionResult> Create(CreateCommunityViewModel model, IFormFile formFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var ms = new MemoryStream();

            await formFile.CopyToAsync(ms);

            model.Photo = ms.ToArray();

            model.PhotoLenght = model.Photo.Length;

            await communityService.CreateCommunityAsync(model);

            return RedirectToAction(nameof(Mine));
        }

        [HttpGet]
        public async Task<IActionResult> Manage(Guid id)
        {
            var community = await communityService.GetCommunityByIdAsync(id);

            if (!await communityService.CheckCommunityCreatorId(id, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (community.Name == null)
            {
                return RedirectToAction("Error404", "Home");
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
        public async Task<IActionResult> Manage(Guid id, IFormFile formFile, CreateCommunityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var ms = new MemoryStream();

            await formFile.CopyToAsync(ms);

            model.Photo = ms.ToArray();

            model.PhotoLenght = model.Photo.Length;

            await communityService.ManageCommunityAsync(id, model);

            return RedirectToAction(nameof(Mine));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await communityService.CheckCommunityCreatorId(id, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.DeleteCommunityAsync(id);

            return RedirectToAction(nameof(Mine));
        }

        public async Task<IActionResult> Restore(Guid id)
        {
            if (!await communityService.CheckCommunityCreatorId(id, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.RestoreCommunityAsync(id);

            return RedirectToAction(nameof(Mine));
        }

        public async Task<IActionResult> DeleteMarketplace(Guid id, Guid communityId)
        {
            if (!await communityService.CheckCommunityCreatorId(communityId, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.DeleteMarketplaceAsync(id);

            return RedirectToAction(nameof(Open), new { id = communityId });
        }

        public async Task<IActionResult> RestoreMarketplace(Guid id, Guid communityId)
        {
            if (!await communityService.CheckCommunityCreatorId(communityId, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.RestoreMarketplaceAsync(id);

            return RedirectToAction(nameof(Open), new { id = communityId });
        }

        public async Task<IActionResult> DeleteChatroom(Guid id, Guid communityId)
        {
            if (!await communityService.CheckCommunityCreatorId(communityId, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.DeleteChatroomAsync(id);

            return RedirectToAction(nameof(Open), new { id = communityId });
        }

        public async Task<IActionResult> RestoreChatroom(Guid id, Guid communityId)
        {
            if (!await communityService.CheckCommunityCreatorId(communityId, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.RestoreChatroomAsync(id);

            return RedirectToAction(nameof(Open), new { id = communityId });
        }
    }
}
