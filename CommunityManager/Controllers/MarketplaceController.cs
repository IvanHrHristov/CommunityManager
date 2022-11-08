using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Extensions;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CommunityManager.Infrastructure.Data.Constants.MessageConstants;

namespace CommunityManager.Controllers
{
    [Authorize]
    public class MarketplaceController : Controller
    {
        private readonly IRepository repository;
        private readonly IMarketplaceServices marketplaceService;
        private readonly UserManager<ApplicationUser> userManager;

        public MarketplaceController(
            IRepository repository,
            IMarketplaceServices service,
            UserManager<ApplicationUser> userManager)
        {
            this.marketplaceService = service;
            this.userManager = userManager;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> All(Guid id)
        {
            var model = await marketplaceService.GetAllAsync();

            ViewBag.Title = "All Products";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Mine()
        {
            var sellerId = User.Id();

            var model = await marketplaceService.GetMineAsync(sellerId);

            ViewBag.Title = "My Products";

            return View("All", model);
        }

        [HttpGet]
        public async Task<IActionResult> Sell()
        {
            var user = await userManager.GetUserAsync(User);

            ViewBag.SellerId = user.Id;

            var model = new ManageProductViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Sell(ManageProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await marketplaceService.SellProductAsync(model);

                return RedirectToAction(nameof(All));
            }
            catch (Exception)
            {

                ModelState.AddModelError("", "Something went wrong.");

                return View(model);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            if (product == null)
            {
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction(nameof(All));
            }

            ManageProductViewModel model = new ManageProductViewModel()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, ManageProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await marketplaceService.EditProducAsync(id, model);

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            if (product == null)
            {
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction(nameof(All));
            }

            await marketplaceService.DeleteProductAsync(id);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await marketplaceService.GetProductByIdAsync(id);

            if (product.Name == null)
            {
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction(nameof(All));
            }

            return View(product);
        }

        public async Task<IActionResult> Buy(Guid id)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            if (product == null)
            {
                TempData[ErrorMessage] = "Incorrect product ID";

                return RedirectToAction(nameof(All));
            }

            var buyerId = User.Id();

            await marketplaceService.BuyProductAsync(id, buyerId);

            return RedirectToAction(nameof(All));
        }
    }
}
