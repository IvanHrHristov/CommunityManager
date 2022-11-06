using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    [Authorize]
    public class MarketplaceController : Controller
    {
        private readonly IMarketplaceServices service;

        public MarketplaceController(IMarketplaceServices service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await service.GetAllAsync();

            return View(model);
        }

        [HttpGet]
        public IActionResult Sell()
        {
            var model = new SellProductViewModel()
            {
                Seller = User.Identity.Name
            };

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
