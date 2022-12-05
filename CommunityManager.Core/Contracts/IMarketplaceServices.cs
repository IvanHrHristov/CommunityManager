using CommunityManager.Core.Models.Marketplace;

namespace CommunityManager.Core.Contracts
{
    public interface IMarketplaceServices
    {
        Task<IEnumerable<ProductsQueryModel>> GetAllAsync(Guid marketplaceId, Guid communityId);

        Task<IEnumerable<ProductsQueryModel>> GetMineAsync(string id, Guid marketplaceId);

        Task<DetailsProductViewModel> GetProductByIdAsync(Guid id);

        Task SellProductAsync(ManageProductViewModel model);

        Task DeleteProductAsync(Guid id);

        Task BuyProductAsync(Guid id, string buyerId);

        Task EditProducAsync(Guid id, ManageProductViewModel model);

        Task<bool> MarketplaceExists(Guid id, Guid communityId);

        Task<IEnumerable<ShoppingCartViewModel>> GetProductsForShoppingCartAsync(string buyerId);

        Task RemoveFromShoppingCartAsync(Guid id);
    }
}
