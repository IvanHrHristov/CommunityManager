using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    /// <summary>
    /// Implementation of marketplace service methods
    /// </summary>
    public class MarketplaceServices : IMarketplaceServices
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;

        public MarketplaceServices(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Creates a new product 
        /// </summary>
        /// <param name="model">Mange product view model</param>
        /// <param name="fileBytes">Photo for the product</param>
        public async Task SellProductAsync(ManageProductViewModel model, byte[] fileBytes)
        {
            var entity = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                SellerId = model.SellerId,
                MarketplaceId = model.MarketplaceId,
                Photo = fileBytes,
                PhotoLenght = fileBytes.Length,
                IsActive = true
            };

            await repository.AddAsync(entity);
            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if a marketplace exists
        /// </summary>
        /// <param name="id">ID of the marketplace</param>
        /// <param name="communityId">ID of the community</param>
        public async Task<bool> MarketplaceExists(Guid id, Guid communityId)
        {
            var marketplace = repository.AllReadonly<Marketplace>()
                .Include(m => m.Community)
                .Where(m => m.CommunityId == communityId &&
                    m.Community.IsActive == true &&
                    m.IsActive == true);

            return await marketplace.AnyAsync(m => m.Id == id);
        }

        /// <summary>
        /// Gets all products in a marketplace
        /// </summary>
        /// <param name="marketplaceId">ID of the marketplace</param>
        /// <param name="communityId">ID of the community</param>
        /// <returns>IEnumerable of products query view models</returns>
        public async Task<IEnumerable<ProductsQueryModel>> GetAllAsync(Guid marketplaceId, Guid communityId)
        {
            var entities = await repository.All<Product>()
                .Include(p => p.Seller)
                .Include(p => p.Marketplace)
                .Where(p => p.BuyerId == null &&
                    p.MarketplaceId == marketplaceId &&
                    p.Marketplace.CommunityId == communityId &&
                    p.IsActive == true)
                .ToListAsync();

            return entities.Select(p => new ProductsQueryModel()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Seller = p?.Seller?.UserName,
                Photo = p.Photo,
                PhotoLenght = p.PhotoLenght
            });
        }

        /// <summary>
        /// Gets all products in a marketplace sold by a user
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <param name="marketplaceId">ID of the marketplace</param>
        /// <returns>IEnumerable of products query view models</returns>
        public async Task<IEnumerable<ProductsQueryModel>> GetMineAsync(string id, Guid marketplaceId)
        {
            var entities = await repository.All<Product>()
                .Include(p => p.Seller)
                .Include(p => p.Buyer)
                .Where(p => p.SellerId == id &&
                    p.MarketplaceId == marketplaceId)
                .ToListAsync();

            return entities.Select(p => new ProductsQueryModel()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Seller = p?.Seller?.UserName,
                Buyer = p?.Buyer?.UserName,
                BuyerId = p?.Buyer?.Id,
                Photo = p.Photo,
                PhotoLenght = p.PhotoLenght,
                IsActive = p.IsActive
            });
        }

        /// <summary>
        /// Sets a product's IsActive to false
        /// </summary>
        /// <param name="id">ID of the product</param>
        public async Task DeleteProductAsync(Guid id)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            product.IsActive = false;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a product with a specific ID
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <returns>Details product view model</returns>
        public async Task<DetailsProductViewModel> GetProductByIdAsync(Guid id)
        {
            var entity = await repository.All<Product>()
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
            {
                return new DetailsProductViewModel();
            }

            return new DetailsProductViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                Photo = entity.Photo,
                PhotoLenght = entity.PhotoLenght,
                Description = entity.Description,
                Seller = entity?.Seller?.UserName
            };
        }

        /// <summary>
        /// Edits the details of a product
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="model">Manage product view model</param>
        /// <param name="fileBytes">Photo of the product</param>
        public async Task EditProducAsync(Guid id, ManageProductViewModel model, byte[] fileBytes)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Photo = fileBytes;
            product.PhotoLenght = fileBytes.Length;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Sets product's BuyerId to the current user ID
        /// </summary>
        /// <param name="id">ID of the product</param>
        /// <param name="buyerId">ID of the user</param>
        public async Task BuyProductAsync(Guid id, string buyerId)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            product.BuyerId = buyerId;

            await repository.SaveChangesAsync();
        }
    }
}
