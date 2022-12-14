using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.RoleConstants;

namespace CommunityManager.Controllers
{
    /// <summary>
    /// Controller to manage communities
    /// </summary>
    [Authorize]
    public class CommunityController : Controller
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;
        /// <summary>
        /// Service providing methods to manage communities
        /// </summary>
        private readonly ICommunityServices communityService;
        /// <summary>
        /// Providing access to the UserManager 
        /// </summary>
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

        /// <summary>
        /// Shows all communities
        /// </summary>
        /// <param name="query">Community query view model</param>
        /// <returns>Community query view model</returns>
        public async Task<IActionResult> All([FromQuery]AllCommunitiesQueryModel query)
        {
            var user = await repository.GetByIdAsync<ApplicationUser>(User.Id());

            var currentUserId = user.Id;

            var model = await communityService.GetAllAsync(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCommunitiesQueryModel.CommunitiesPerPage,
                currentUserId);

            query.Communities = model.Communities;
            query.TotalCommunitiesCount = model.TotalCommunities;

            ViewBag.currentUserId = currentUserId;

            var stringArray = new string[model.Communities.Count()];
            int i = 0;

            foreach (var community in model.Communities)
            {
                stringArray[i] = "data:image/png;base64," + Convert.ToBase64String(community.Photo, 0, community.PhotoLenght);
                i++;
            }

            ViewBag.CurrentUserAge = user.Age;
            ViewBag.Base64StringCollection = stringArray;

            return View(query);
        }

        /// <summary>
        /// Adds a user to a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Redirects the user to the view showing all communities</returns>
        [HttpGet]
        public async Task<IActionResult> Join(Guid id)
        {
            var currentUserId = User.Id();

            var community = await repository.GetByIdAsync<Community>(id);
            var user = await repository.GetByIdAsync<ApplicationUser>(currentUserId);

            if (await communityService.CheckCommunityMemberId(id, currentUserId))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (community == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            if (user == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            if (community.AgeRestricted)
            {
                if (user.Age < 18)
                {
                    return RedirectToAction("Error404", "Home");
                }
            }

            await communityService.JoinCommunityAsync(id, currentUserId);

            return RedirectToAction(nameof(All));
        }

        /// <summary>
        /// Shows all communities that you are a member in
        /// </summary>
        /// <returns>Collection of community view models</returns>
        public async Task<IActionResult> Mine()
        {
            var currentUserId = User.Id();

            var model = await communityService.GetMineAsync(currentUserId);

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

            if (!(await communityService.CheckCommunityMemberId(id, currentUserId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            var model = await communityService.GetCommunityByIdAsync(id);
            
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
            if (!(await communityService.CheckCommunityCreatorId(id, User.Id())))
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
            if (!(await communityService.CheckCommunityCreatorId(id, User.Id())))
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

            var user = await userManager.GetUserAsync(User);

            await userManager.AddToRoleAsync(user, Administrator);

            return RedirectToAction(nameof(Mine));
        }

        /// <summary>
        /// Shows details for a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>The view to show details of a community</returns>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var currentUserId = User.Id();

            ViewBag.currentUserId = currentUserId;

            var model = await communityService.GetCommunityByIdAsync(id);

            if (!(await communityService.CheckCommunityMemberId(id, currentUserId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (model.Name == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            ViewBag.Base64String = "data:image/png;base64," + Convert.ToBase64String(model.Photo, 0, model.PhotoLenght);
            
            return View(model);
        }

        /// <summary>
        /// Redirects the user to the view to edit a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>The view to manage a community</returns>
        [HttpGet]
        public async Task<IActionResult> Manage(Guid id)
        {
            var community = await repository.GetByIdAsync<Community>(id);

            if (!(await communityService.CheckCommunityCreatorId(id, User.Id())))
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
        public async Task<IActionResult> Manage(CreateCommunityViewModel model, IFormFile formFile, Guid id)
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
        /// Remove a user from a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Redirects the user to the view with their communities</returns>
        public async Task<IActionResult> Leave(Guid id)
        {
            var userId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(id, userId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            await communityService.LeaveCommunityAsync(id, userId);

            return RedirectToAction(nameof(Mine));
        }
    }
}
