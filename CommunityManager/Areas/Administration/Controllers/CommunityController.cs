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
    /// <summary>
    /// Controller to manage communities for the administration area
    /// </summary>
    [Authorize(Roles = Administrator)]
    [Area(AdminArea)]
    public class CommunityController : Controller
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;
        /// <summary>
        /// Service for the community controller
        /// </summary>
        private readonly ICommunityServices communityService;

        public CommunityController(
            IRepository repository,
            ICommunityServices communityService)
        {
            this.repository = repository;
            this.communityService = communityService;
        }
        
        /// <summary>
        /// Shows all communities that you are a member in
        /// </summary>
        /// <returns>Collection of community view models</returns>
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

        /// <summary>
        /// Opens a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Community view model</returns>
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

        /// <summary>
        /// Redirects user to the view to add a marketplace to a community
        /// </summary>
        /// <returns>The view to add a marketplace</returns>
        [HttpGet]
        public IActionResult AddMarketplace()
        {
            var model = new AddMarketplaceViewModel();

            return View(model);
        }

        /// <summary>
        /// Adds a marketplace to a community
        /// </summary>
        /// <param name="model">Marketplace view model</param>
        /// <param name="id">ID of the community</param>
        /// <returns>Redirects the user to the community</returns>
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

        /// <summary>
        /// Redirects user to the view to add a chatroom to a community
        /// </summary>
        /// <returns>The view to add a chatroom</returns>
        [HttpGet]
        public IActionResult AddChatroom()
        {
            var model = new AddChatroomViewModel();

            return View(model);
        }

        /// <summary>
        /// Adds a chatroom to a community
        /// </summary>
        /// <param name="model">Chatroom view model</param>
        /// <param name="id">ID of the community</param>
        /// <returns>Redirects the user to the community</returns>
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

        /// <summary>
        /// Redirects the user to the view to create a new community
        /// </summary>
        /// <returns>The view to create a community</returns>
        [HttpGet]
        public IActionResult Create()
        {
            var creatorId = User.Id();

            ViewBag.CreatorId = creatorId;

            var model = new CreateCommunityViewModel();

            return View(model);
        }

        /// <summary>
        /// Creates a new community
        /// </summary>
        /// <param name="model">Community view model</param>
        /// <param name="formFile">A photo to be used as cover for that community</param>
        /// <returns>Redirects the user to the view with their communities</returns>
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

        /// <summary>
        /// Redirects the user to the view to edit a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>The view to manage a community</returns>
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

        /// <summary>
        /// Edits the details of a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <param name="formFile">A photo to be used as a cover for the community</param>
        /// <param name="model">Community view model</param>
        /// <returns>Redirects the user to the view with their communities</returns>
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

        /// <summary>
        /// Sets IsActive for a community to false
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Redirects the user to the view with their communities</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await communityService.CheckCommunityCreatorId(id, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.DeleteCommunityAsync(id);

            return RedirectToAction(nameof(Mine));
        }

        /// <summary>
        /// Sets IsActive for a community to true
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Redirects the user to the view with their communities</returns>
        public async Task<IActionResult> Restore(Guid id)
        {
            if (!await communityService.CheckCommunityCreatorId(id, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.RestoreCommunityAsync(id);

            return RedirectToAction(nameof(Mine));
        }

        /// <summary>
        /// Deletes a marketplace 
        /// </summary>
        /// <param name="id">ID of the marketplace</param>
        /// <param name="communityId">ID of the community that the marketplace is in</param>
        /// <returns>Redirects the user to the community</returns>
        public async Task<IActionResult> DeleteMarketplace(Guid id, Guid communityId)
        {
            if (!await communityService.CheckCommunityCreatorId(communityId, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.DeleteMarketplaceAsync(id);

            return RedirectToAction(nameof(Open), new { id = communityId });
        }

        /// <summary>
        /// Restores a marketplace 
        /// </summary>
        /// <param name="id">ID of the marketplace</param>
        /// <param name="communityId">ID of the community that the marketplace is in</param>
        /// <returns>Redirects the user to the community</returns>
        public async Task<IActionResult> RestoreMarketplace(Guid id, Guid communityId)
        {
            if (!await communityService.CheckCommunityCreatorId(communityId, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.RestoreMarketplaceAsync(id);

            return RedirectToAction(nameof(Open), new { id = communityId });
        }

        /// <summary>
        /// Deletes a chatroom 
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="communityId">ID of the community that the chatroom is in</param>
        /// <returns>Redirects the user to the community</returns>
        public async Task<IActionResult> DeleteChatroom(Guid id, Guid communityId)
        {
            if (!await communityService.CheckCommunityCreatorId(communityId, User.Id()))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.DeleteChatroomAsync(id);

            return RedirectToAction(nameof(Open), new { id = communityId });
        }

        /// <summary>
        /// Restores a chatroom 
        /// </summary>
        /// <param name="id">ID of the chatroom</param>
        /// <param name="communityId">ID of the community that the chatroom is in</param>
        /// <returns>Redirects the user to the community</returns>
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
