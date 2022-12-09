using CommunityManager.Core.Contracts;
using CommunityManager.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartServices shoppingCartService;

        public ShoppingCartController(IShoppingCartServices shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await shoppingCartService.GetProductsAsync(User.Id());

            var stringArray = new string[model.Items.Count()];
            int i = 0;

            foreach (var product in model.Items)
            {
                stringArray[i] = "data:image/png;base64," + Convert.ToBase64String(product.Photo, 0, product.PhotoLenght);
                i++;
            }

            ViewBag.Base64StringCollection = stringArray;

            return View(model);
        }

        public async Task<IActionResult> Remove(Guid id)
        {
            await shoppingCartService.RemoveAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Pay()
        {
            await shoppingCartService.PayAsync(User.Id());

            return RedirectToAction("Index", "Home");
        }
    }
}
