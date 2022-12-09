using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    [Authorize]
    public class MarketplaceController : Controller
    {
        private readonly IRepository repository;
        private readonly IMarketplaceServices marketplaceService;
        private readonly ICommunityServices communityService;
        private readonly UserManager<ApplicationUser> userManager;

        public MarketplaceController(
            IRepository repository,
            IMarketplaceServices service,
            UserManager<ApplicationUser> userManager,
            ICommunityServices communityService)
        {
            this.marketplaceService = service;
            this.userManager = userManager;
            this.repository = repository;
            this.communityService = communityService;
        }

        [HttpGet]
        public async Task<IActionResult> All(Guid id, Guid communityId)
        {
            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            if (!(await marketplaceService.MarketplaceExists(id, communityId)))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            var model = await marketplaceService.GetAllAsync(id, communityId);

            ViewBag.Title = "All Products";
            ViewBag.CommunityId = communityId;

            var stringArray = new string[model.Count()];
            int i = 0;

            foreach (var product in model)
            {
                stringArray[i] = "data:image/png;base64," + Convert.ToBase64String(product.Photo, 0, product.PhotoLenght);
                i++;
            }

            ViewBag.Base64StringCollection = stringArray;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Mine(Guid id, Guid communityId)
        {
            var sellerId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(communityId, sellerId)))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            if (!(await marketplaceService.MarketplaceExists(id, communityId)))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            var model = await marketplaceService.GetMineAsync(sellerId, id);

            ViewBag.Title = "My Products";
            ViewBag.CommunityId = communityId;

            var stringArray = new string[model.Count()];
            int i = 0;

            foreach (var product in model)
            {
                stringArray[i] = "data:image/png;base64," + Convert.ToBase64String(product.Photo, 0, product.PhotoLenght);
                i++;
            }

            ViewBag.Base64StringCollection = stringArray;

            return View("All", model);
        }

        [HttpGet]
        public async Task<IActionResult> Sell(Guid id, Guid communityId)
        {
            var sellerId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(communityId, sellerId)))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            if (!(await marketplaceService.MarketplaceExists(id, communityId)))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            ViewBag.SellerId = sellerId;
            ViewBag.MarketplaceId = id;
            ViewBag.CommunityId = communityId;

            var model = new ManageProductViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Sell(IFormFile formFile, ManageProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!(await communityService.CheckCommunityMemberId(model.CommunityId, User.Id())))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = model.CommunityId, manageErrorMessage = errorMessage });
            }

            if (!(await marketplaceService.MarketplaceExists(model.MarketplaceId, model.CommunityId)))
            {
                var errorMessage = "An error occured while trying to open that chatroom";

                return RedirectToAction("Open", "Community", new { id = model.CommunityId, manageErrorMessage = errorMessage });
            }

            using var ms = new MemoryStream();

            await formFile.CopyToAsync(ms);

            var fileBytes = ms.ToArray();

            await marketplaceService.SellProductAsync(model, fileBytes);

            return RedirectToAction(nameof(All), new { id = model.MarketplaceId, communityId = model.CommunityId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, Guid communityId)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                var errorMessage = "An error occured while trying to edit that product";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            if (product == null)
            {
                var errorMessage = "An error occured while trying to edit that product";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            ManageProductViewModel model = new ManageProductViewModel()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };

            ViewBag.SellerId = product.SellerId;
            ViewBag.MarketplaceId = product.MarketplaceId;
            ViewBag.CommunityId = communityId;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, ManageProductViewModel model, IFormFile formFile)
        {
            if (!(await communityService.CheckCommunityMemberId(model.CommunityId, User.Id())))
            {
                var errorMessage = "An error occured while trying to edit that product";

                return RedirectToAction("Open", "Community", new { id = model.CommunityId, manageErrorMessage = errorMessage });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var ms = new MemoryStream();

            await formFile.CopyToAsync(ms);

            var fileBytes = ms.ToArray();

            await marketplaceService.EditProducAsync(id, model, fileBytes);

            return RedirectToAction(nameof(All), new { id = model.MarketplaceId, communityId = model.CommunityId });
        }

        public async Task<IActionResult> Delete(Guid id, Guid communityId)
        {
            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                var errorMessage = "An error occured while trying to edit that product";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            var product = await repository.GetByIdAsync<Product>(id);

            if (product == null)
            {
                var errorMessage = "An error occured while trying to delete that product";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            await marketplaceService.DeleteProductAsync(id);

            return RedirectToAction(nameof(Mine), new { id = product.MarketplaceId, communityId  = communityId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, Guid communityId)
        {
            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                var errorMessage = "An error occured while trying to view details about that product";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            var product = await marketplaceService.GetProductByIdAsync(id);

            if (product.Name == null)
            {
                var errorMessage = "An error occured while trying to view details about that product";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            ViewBag.CommunityId = communityId;

            ViewBag.Base64String = "data:image/png;base64," + Convert.ToBase64String(product.Photo, 0, product.PhotoLenght); ;

            return View(product);
        }

        public async Task<IActionResult> Buy(Guid id, Guid communityId)
        {
            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                var errorMessage = "An error occured while trying to buy that product";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            var product = await repository.GetByIdAsync<Product>(id);

            if (product == null)
            {
                var errorMessage = "An error occured while trying to buy that product";

                return RedirectToAction("Open", "Community", new { id = communityId, manageErrorMessage = errorMessage });
            }

            var buyerId = User.Id();

            await marketplaceService.BuyProductAsync(id, buyerId);

            return RedirectToAction(nameof(All), new { id = product.MarketplaceId, communityId = communityId });
        }
    }
}
