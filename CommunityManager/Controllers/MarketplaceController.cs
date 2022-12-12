using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    /// <summary>
    /// Controller to manage marketplaces
    /// </summary>
    [Authorize]
    public class MarketplaceController : Controller
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;
        /// <summary>
        /// Service providing methods to manage marketplaces
        /// </summary>
        private readonly IMarketplaceServices marketplaceService;
        /// <summary>
        /// Service providing methods to manage communities
        /// </summary>
        private readonly ICommunityServices communityService;

        public MarketplaceController(
            IRepository repository,
            IMarketplaceServices service,
            ICommunityServices communityService)
        {
            this.marketplaceService = service;
            this.repository = repository;
            this.communityService = communityService;
        }

        /// <summary>
        /// Shows all products in a marketplace
        /// </summary>
        /// <param name="id">ID of the marketplace</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Products query view model</returns>
        [HttpGet]
        public async Task<IActionResult> All(Guid id, Guid communityId)
        {
            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (!(await marketplaceService.MarketplaceExists(id, communityId)))
            {
                return RedirectToAction("Error404", "Home");
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

        /// <summary>
        /// Shows all products you are selling in a marketplace
        /// </summary>
        /// <param name="id">ID of the marketplace</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Products query view model</returns>
        [HttpGet]
        public async Task<IActionResult> Mine(Guid id, Guid communityId)
        {
            var sellerId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(communityId, sellerId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (!(await marketplaceService.MarketplaceExists(id, communityId)))
            {
                return RedirectToAction("Error404", "Home");
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

        /// <summary>
        /// Redirects the user to the sell product view 
        /// </summary>
        /// <param name="id">ID of the marketplace</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Manage products view model</returns>
        [HttpGet]
        public async Task<IActionResult> Sell(Guid id, Guid communityId)
        {
            var sellerId = User.Id();

            if (!(await communityService.CheckCommunityMemberId(communityId, sellerId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (!(await marketplaceService.MarketplaceExists(id, communityId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            ViewBag.SellerId = sellerId;
            ViewBag.MarketplaceId = id;
            ViewBag.CommunityId = communityId;

            var model = new ManageProductViewModel();

            return View(model);
        }

        /// <summary>
        /// Adds a product to a marketplace
        /// </summary>
        /// <param name="formFile">Photo for the product</param>
        /// <param name="model">Manage product view model</param>
        /// <returns>Redirects the user to the view with all products in a marketplace</returns>
        [HttpPost]
        public async Task<IActionResult> Sell(IFormFile formFile, ManageProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!(await communityService.CheckCommunityMemberId(model.CommunityId, User.Id())))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (!(await marketplaceService.MarketplaceExists(model.MarketplaceId, model.CommunityId)))
            {
                return RedirectToAction("Error404", "Home");
            }

            using var ms = new MemoryStream();

            await formFile.CopyToAsync(ms);

            var fileBytes = ms.ToArray();

            await marketplaceService.SellProductAsync(model, fileBytes);

            return RedirectToAction(nameof(All), new { id = model.MarketplaceId, communityId = model.CommunityId });
        }

        /// <summary>
        /// Redirects the user to the view to edit a product
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Manage product view model</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, Guid communityId)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                return RedirectToAction("Error404", "Home");
            }

            if (product == null)
            {
                return RedirectToAction("Error404", "Home");
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

        /// <summary>
        /// Edits the details of a product
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="model">Manage product view model</param>
        /// <param name="formFile">Photo for the product</param>
        /// <returns>Redirects the user to the view with all products in a marketplace</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, ManageProductViewModel model, IFormFile formFile)
        {
            if (!(await communityService.CheckCommunityMemberId(model.CommunityId, User.Id())))
            {
                return RedirectToAction("Error404", "Home");
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

        /// <summary>
        /// Removes a product
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Redirects the user to their products in a marketplace</returns>
        public async Task<IActionResult> Delete(Guid id, Guid communityId)
        {
            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                return RedirectToAction("Error404", "Home");
            }

            var product = await repository.GetByIdAsync<Product>(id);

            if (product == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            await marketplaceService.DeleteProductAsync(id);

            return RedirectToAction(nameof(Mine), new { id = product.MarketplaceId, communityId  = communityId });
        }

        /// <summary>
        /// Shows the details of a product
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Details product view model</returns>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, Guid communityId)
        {
            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                return RedirectToAction("Error404", "Home");
            }

            var product = await marketplaceService.GetProductByIdAsync(id);

            if (product.Name == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            ViewBag.CommunityId = communityId;

            ViewBag.Base64String = "data:image/png;base64," + Convert.ToBase64String(product.Photo, 0, product.PhotoLenght); ;

            return View(product);
        }

        /// <summary>
        /// Sets the product BuyerId to the current user ID
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>Redirects the user to the view with all products in a marketplace</returns>
        public async Task<IActionResult> Buy(Guid id, Guid communityId)
        {
            if (!(await communityService.CheckCommunityMemberId(communityId, User.Id())))
            {
                return RedirectToAction("Error404", "Home");
            }

            var product = await repository.GetByIdAsync<Product>(id);

            if (product == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            if (product.SellerId == User.Id())
            {
                return RedirectToAction("Error404", "Home");
            }

            var buyerId = User.Id();

            await marketplaceService.BuyProductAsync(id, buyerId);

            return RedirectToAction(nameof(All), new { id = product.MarketplaceId, communityId = communityId });
        }
    }
}