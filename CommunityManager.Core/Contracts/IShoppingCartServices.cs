using CommunityManager.Core.Models.ShoppingCart;

namespace CommunityManager.Core.Contracts
{
    /// <summary>
    /// Abstraction of community service methods
    /// </summary>
    public interface IShoppingCartServices
    {
        /// <summary>
        /// Gets all products in a shopping cart
        /// </summary>
        /// <param name="buyerId">ID of the current user</param>
        /// <returns>Shopping cart view model</returns>
        Task<ShoppingCartViewModel> GetProductsAsync(string buyerId);

        /// <summary>
        /// Removes a product from a shopping cart
        /// </summary>
        /// <param name="id">ID of the product</param>
        Task RemoveAsync(Guid id);

        /// <summary>
        /// Removes all products in a shopping cart
        /// </summary>
        /// <param name="buyerId">ID of the current user</param>
        Task PayAsync(string buyerId);
    }
}
