using CommunityManager.Core.Models.Marketplace;

namespace CommunityManager.Core.Contracts
{
    /// <summary>
    /// Abstraction of marketplace service methods
    /// </summary>
    public interface IMarketplaceServices
    {
        /// <summary>
        /// Gets all products in a marketplace
        /// </summary>
        /// <param name="marketplaceId">ID of the marketplace</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>IEnumerable of products query view models</returns>
        Task<IEnumerable<ProductsQueryModel>> GetAllAsync(Guid marketplaceId, Guid communityId);

        /// <summary>
        /// Gets all products in a marketplace sold by a user
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <param name="marketplaceId">ID of the marketplace</param>
        /// <returns>IEnumerable of products query view models</returns>
        Task<IEnumerable<ProductsQueryModel>> GetMineAsync(string id, Guid marketplaceId);

        /// <summary>
        /// Gets a product with a specific ID
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <returns>Details product view model</returns>
        Task<DetailsProductViewModel> GetProductByIdAsync(Guid id);

        /// <summary>
        /// Creates a new product 
        /// </summary>
        /// <param name="model">Mange product view model</param>
        /// <param name="fileBytes">Photo for the product</param>
        Task SellProductAsync(ManageProductViewModel model, byte[] fileBytes);

        /// <summary>
        /// Sets a product's IsActive to false
        /// </summary>
        /// <param name="id">ID of the product</param>
        Task DeleteProductAsync(Guid id);

        /// <summary>
        /// Sets product's BuyerId to the current user ID
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="buyerId">ID of the user</param>
        Task BuyProductAsync(Guid id, string buyerId);

        /// <summary>
        /// Edits the details of a product
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="model">Manage product view model</param>
        /// <param name="fileBytes">Photo of the product</param>
        Task EditProducAsync(Guid id, ManageProductViewModel model, byte[] fileBytes);

        /// <summary>
        /// Checks if a marketplace exists
        /// </summary>
        /// <param name="id">ID of the marketplace</param>
        /// <param name="communityId">ID of the community</param>
        Task<bool> MarketplaceExists(Guid id, Guid communityId);
    }
}
