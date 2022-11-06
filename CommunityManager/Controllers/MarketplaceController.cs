using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    [Authorize]
    public class MarketplaceController : Controller
    {
        private readonly IMarketplaceServices service;
        private readonly UserManager<ApplicationUser> userManager;

        public MarketplaceController(
            IMarketplaceServices service,
            UserManager<ApplicationUser> userManager)
        {
            this.service = service;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await service.GetAllAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Sell()
        {
            var user = await userManager.GetUserAsync(User);

            ViewBag.SellerId = user.Id;

            var model = new SellProductViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Sell(SellProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await service.SellProductAsync(model);

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
