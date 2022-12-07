﻿using CommunityManager.Core.Contracts;
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

        public MarketplaceServices(IRepository repository)
        {
            this.repository = repository;
        }

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

        public async Task<bool> MarketplaceExists(Guid id, Guid communityId)
        {
            var marketplace = repository.AllReadonly<Marketplace>()
                .Include(m => m.Community)
                .Where(m => m.CommunityId == communityId &&
                    m.Community.IsActive == true &&
                    m.IsActive == true);

            return await marketplace.AnyAsync(m => m.Id == id);
        }

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

        public async Task<IEnumerable<ProductsQueryModel>> GetMineAsync(string id, Guid marketplaceId)
        {
            var entities = await repository.All<Product>()
                .Include(p => p.Seller)
                .Include(p => p.Buyer)
                .Where(p => p.SellerId == id &&
                    p.MarketplaceId == marketplaceId &&
                    p.IsActive == true)
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
                PhotoLenght = p.PhotoLenght
            });
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            product.IsActive = false;

            //await repository.DeleteAsync<Product>(id);

            await repository.SaveChangesAsync();
        }

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

        public async Task BuyProductAsync(Guid id, string buyerId)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            product.BuyerId = buyerId;

            await repository.SaveChangesAsync();
        }

        public async Task<ShoppingCartViewModel> GetProductsForShoppingCartAsync(string buyerId)
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

        public async Task RemoveFromShoppingCartAsync(Guid id)
        {
            var product = await repository.GetByIdAsync<Product>(id);

            product.BuyerId = null;

            await repository.SaveChangesAsync();
        }

        public async Task PayForProductsInShoppingCartAsync(string buyerId)
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
