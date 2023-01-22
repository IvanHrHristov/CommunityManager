using CommunityManager.Core.Contracts;
using CommunityManager.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CommunityManager.Controllers
{
    /// <summary>
    /// Controller to manage shopping carts
    /// </summary>
    public class ShoppingCartController : Controller
    {
        /// <summary>
        /// Service providing methods to manage shopping carts
        /// </summary>
        private readonly IShoppingCartServices shoppingCartService;

        public ShoppingCartController(IShoppingCartServices shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        /// <summary>
        /// Shows all items in a shopping cart
        /// </summary>
        /// <returns>Shopping cart view model</returns>
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

        /// <summary>
        /// Remove product from a shopping cart
        /// </summary>
        /// <param name="id">ID of the product</param>
        public async Task<IActionResult> Remove(Guid id)
        {
            await shoppingCartService.RemoveAsync(id);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Removes all products from the shopping cart
        /// </summary>
        public async Task<IActionResult> Pay()
        {
            await shoppingCartService.PayAsync(User.Id());

            return RedirectToAction("Index", "Home");
        }
    }
}
