using CommunityManager.Core.Models.ShoppingCart;

namespace CommunityManager.Core.Contracts
{
    public interface IShoppingCartServices
    {
        Task<ShoppingCartViewModel> GetProductsAsync(string buyerId);

        Task RemoveAsync(Guid id);

        Task PayAsync(string buyerId);
    }
}
