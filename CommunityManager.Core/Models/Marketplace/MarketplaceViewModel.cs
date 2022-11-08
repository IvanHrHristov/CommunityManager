using CommunityManager.Infrastructure.Data.Models;

namespace CommunityManager.Core.Models.Marketplace
{
    public class MarketplaceViewModel
    {
        public Guid Id { get; set; }

        public List<ProductsQueryModel>? Products { get; set; } = new List<ProductsQueryModel>();
    }
}
