using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Infrastructure.Data.Models;

namespace CommunityManager.Core.Contracts
{
    public interface IMarketplaceServices
    {
        Task<IEnumerable<ProductsQueryModel>> GetAllAsync();

        Task SellProductAsync(SellProductViewModel model);
    }
}
