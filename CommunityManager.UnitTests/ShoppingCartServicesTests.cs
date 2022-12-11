using CommunityManager.Core.Contracts;
using CommunityManager.Core.Services;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityManager.UnitTests
{
    public class ShoppingCartServicesTests
    {
        private IShoppingCartServices shoppingCartServices;
        private ApplicationDbContext context;

        [SetUp]
        public void Setup()
        {
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("CommunityDB")
                .Options;

            context = new ApplicationDbContext(contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Test]
        public async Task TestGetAll()
        {
            var repository = new Repository(context);
            shoppingCartServices = new ShoppingCartServices(repository);

            string buyerId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";
            string sellerId = "cee1a987-641c-45b9-a2a0-2c785ac72362";

            await repository.AddAsync(new Product()
            {
                SellerId = sellerId,
                Description = "",
                BuyerId = buyerId,
                IsActive = true,
                Name = "product1",
                Photo = new Byte[32],
                PhotoLenght = 1,
                Price = 5.5M
            });

            await repository.AddAsync(new Product()
            {
                SellerId = sellerId,
                Description = "",
                BuyerId = buyerId,
                IsActive = true,
                Name = "product2",
                Photo = new Byte[32],
                PhotoLenght = 1,
                Price = 3M
            });

            await repository.SaveChangesAsync();

            var products = await shoppingCartServices.GetProductsAsync(buyerId);

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Items.Count(), Is.EqualTo(2));
            Assert.That(products.TotalPrice, Is.EqualTo(8.5M));
        }

        [Test]
        public async Task TestRemove()
        {
            var repository = new Repository(context);
            shoppingCartServices = new ShoppingCartServices(repository);

            Guid productId = Guid.Parse("9523b00b-e4cf-4c29-8ff4-cb2ca968fc3d");
            string buyerId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";
            string sellerId = "cee1a987-641c-45b9-a2a0-2c785ac72362";

            await repository.AddAsync(new Product()
            {
                Id = productId,
                SellerId = sellerId,
                Description = "",
                BuyerId = buyerId,
                IsActive = true,
                Name = "product1",
                Photo = new Byte[32],
                PhotoLenght = 1,
                Price = 5.5M
            });

            await repository.SaveChangesAsync();

            await shoppingCartServices.RemoveAsync(productId);

            var products = await repository.GetByIdAsync<Product>(productId);

            Assert.That(products.BuyerId, Is.Null);
        }

        [Test]
        public async Task TestPay()
        {
            var repository = new Repository(context);
            shoppingCartServices = new ShoppingCartServices(repository);

            Guid productId = Guid.Parse("9523b00b-e4cf-4c29-8ff4-cb2ca968fc3d");
            Guid productId2 = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string buyerId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";
            string sellerId = "cee1a987-641c-45b9-a2a0-2c785ac72362";

            await repository.AddAsync(new Product()
            {
                Id = productId,
                SellerId = sellerId,
                Description = "",
                BuyerId = buyerId,
                IsActive = true,
                Name = "product1",
                Photo = new Byte[32],
                PhotoLenght = 1,
                Price = 5.5M
            });

            await repository.AddAsync(new Product()
            {
                Id = productId2,
                SellerId = sellerId,
                Description = "",
                BuyerId = buyerId,
                IsActive = true,
                Name = "product1",
                Photo = new Byte[32],
                PhotoLenght = 1,
                Price = 5.5M
            });

            await repository.SaveChangesAsync();

            await shoppingCartServices.PayAsync(buyerId);

            var products = await repository.All<Product>()
                .Where(p => p.BuyerId == buyerId)
                .ToListAsync();

            Assert.That(products.All(p => p.IsActive == false), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
