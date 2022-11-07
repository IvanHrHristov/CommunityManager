using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    public class MarketplaceServices : IMarketplaceServices
    {
        private readonly IRepository repository;
        private readonly ApplicationDbContext context;

        public MarketplaceServices(
            IRepository repository,
            ApplicationDbContext context)
        {
            this.context = context;
            this.repository = repository;
        }

        public async Task SellProductAsync(ManageProductViewModel model)
        {
            var entity = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                SellerId = model.SellerId
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
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Seller = p?.Seller?.UserName
            });
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            context.Products.Remove(product);
            context.SaveChanges();
        }

        public async Task<DetailsProductViewModel> GetProductByIdAsync(Guid id)
        {
            var entities = await context.Products
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

            return new DetailsProductViewModel()
            {
                Id = entities.Id,
                Name = entities.Name,
                Price = entities.Price,
                Description = entities.Description,
                ImageUrl = entities.ImageUrl,
                Seller = entities?.Seller?.UserName
            };
        }

        public async Task EditProducAsync(Guid id, ManageProductViewModel model)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;

            await repository.SaveChangesAsync();
        }
    }
}
