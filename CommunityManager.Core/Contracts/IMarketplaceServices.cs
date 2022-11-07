using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Infrastructure.Data.Models;

namespace CommunityManager.Core.Contracts
{
    public interface IMarketplaceServices
    {
        Task<IEnumerable<ProductsQueryModel>> GetAllAsync();

        Task<DetailsProductViewModel> GetProductByIdAsync(Guid id);

        Task SellProductAsync(ManageProductViewModel model);

        Task DeleteProductAsync(Guid id);

        Task EditProducAsync(Guid id, ManageProductViewModel model);
    }
}
