using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    public class MarketplaceServices : IMarketplaceServices
    {
        private readonly ApplicationDbContext context;

        public MarketplaceServices(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task SellProductAsync(SellProductViewModel model)
        {
            var entity = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl
            };

            await context.Products.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductsQueryModel>> GetAllAsync()
        {
            var entities = await context.Products
                .Include(p => p.Seller)
                .ToListAsync();

            return entities.Select(p => new ProductsQueryModel()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Seller = p?.Seller?.UserName
            });
        }
    }
}
