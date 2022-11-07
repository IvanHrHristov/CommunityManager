using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> All()
        {
            var model = await marketplaceService.GetAllAsync();

            return View(model);
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

            await marketplaceService.DeleteProductAsync(id);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await marketplaceService.GetProductByIdAsync(id);

            return View(product);
        }
    }
}
