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
    [Authorize]
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

        [HttpGet]
        public async Task<IActionResult> Open(Guid id)
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

        [HttpGet]
        public IActionResult AddChatroom()
        {
            var model = new AddChatroomViewModel();

            return View(model);
        }

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

            var user = await userManager.GetUserAsync(User);

            await userManager.AddToRoleAsync(user, Administrator);

            return RedirectToAction(nameof(Mine));
        }

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
