﻿using CommunityManager.Core.Models.Marketplace;

namespace CommunityManager.Core.Contracts
{
    public interface IMarketplaceServices
    {
        Task<IEnumerable<ProductsQueryModel>> GetAllAsync();

        Task<IEnumerable<ProductsQueryModel>> GetMineAsync(string id);

        Task<DetailsProductViewModel> GetProductByIdAsync(Guid id);

        Task SellProductAsync(ManageProductViewModel model);

        Task DeleteProductAsync(Guid id);

        Task BuyProductAsync(Guid id, string buyerId);

        Task EditProducAsync(Guid id, ManageProductViewModel model);
    }
}