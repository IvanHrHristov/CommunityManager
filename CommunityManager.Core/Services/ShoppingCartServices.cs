using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Models.ShoppingCart;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    /// <summary>
    /// Implementation of community service methods
    /// </summary>
    public class ShoppingCartServices : IShoppingCartServices
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;

        public ShoppingCartServices(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets all products in a shopping cart
        /// </summary>
        /// <param name="buyerId">ID of the current user</param>
        /// <returns>Shopping cart view model</returns>
        public async Task<ShoppingCartViewModel> GetProductsAsync(string buyerId)
        {
            var product = await repository.All<Product>()
                .Where(p => p.BuyerId == buyerId && p.IsActive == true)
                .ToListAsync();

            decimal totalSum = 0M;

            foreach (var item in product)
            {
                totalSum += item.Price;
            }

            return new ShoppingCartViewModel()
            {
                TotalPrice = totalSum,
                Items = product.Select(p => new ShoppingCartItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Photo = p.Photo,
                    PhotoLenght = p.PhotoLenght
                }).ToList()
            };
        }

        /// <summary>
        /// Removes a product from a shopping cart
        /// </summary>
        /// <param name="id">ID of the product</param>
        public async Task RemoveAsync(Guid id)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            product.BuyerId = null;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Removes all products in a shopping cart
        /// </summary>
        /// <param name="buyerId">ID of the current user</param>
        public async Task PayAsync(string buyerId)
        {
            var product = await repository.All<Product>()
                .Where(p => p.BuyerId == buyerId && p.IsActive == true)
            .ToListAsync();

            foreach (var item in product)
            {
                item.IsActive = false;
            }

            await repository.SaveChangesAsync();
        }
    }
}
