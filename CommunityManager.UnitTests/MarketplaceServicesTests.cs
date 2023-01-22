using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Services;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using CommunityManager.Core.Models.Marketplace;
using System.Linq;

namespace CommunityManager.UnitTests
{
    public class MarketplaceServicesTests
    {
        private IMarketplaceServices marketplaceServices;
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
        public async Task TestSellProduct()
        {
            var repository = new Repository(context);
            marketplaceServices = new MarketplaceServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string sellerId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            var model = new ManageProductViewModel()
            {
                Name = "test",
                Description = "test",
                Price = 1,
                SellerId = sellerId,
                MarketplaceId = marketplaceId
            };

            await marketplaceServices.SellProductAsync(model, new byte[32]);

            var product = await repository.All<Product>().Where(p => p.Name == "test").FirstOrDefaultAsync();

            Assert.That(product, Is.Not.Null);
        }

        [Test]
        public async Task TestMarketplaceExists()
        {
            var repository = new Repository(context);
            marketplaceServices = new MarketplaceServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid communityId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            Guid falseMarketplaceId = Guid.NewGuid();
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Marketplace()
            {
                Id = marketplaceId,
                IsActive = true,
                Name = "",
                CommunityId = communityId
            });

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Name = "",
                Description = "",
                Photo = new byte[32],
                IsActive = true
            });

            await repository.SaveChangesAsync();

            var result = await marketplaceServices.MarketplaceExists(marketplaceId, communityId);
            var falseResult = await marketplaceServices.MarketplaceExists(marketplaceId, falseMarketplaceId);

            Assert.That(result, Is.True);
            Assert.That(falseResult, Is.False);
        }

        [Test]
        public async Task TestGetAll()
        {
            var repository = new Repository(context);
            marketplaceServices = new MarketplaceServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid communityId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new ApplicationUser()
            {
                Id = creatorId,
                IsActive = true
            });

            await repository.AddAsync(new Marketplace()
            {
                Id = marketplaceId,
                IsActive = true,
                Name = "",
                CommunityId = communityId
            });

            await repository.AddAsync(new Product()
            {
                Name = "",
                SellerId = creatorId,
                Description = "",
                IsActive = true,
                MarketplaceId = marketplaceId,
                Photo = new byte[32],
                PhotoLenght = 1,
                Price = 1
            });

            await repository.AddAsync(new Product()
            {
                Name = "",
                SellerId = creatorId,
                Description = "",
                IsActive = true,
                MarketplaceId = marketplaceId,
                Photo = new byte[32],
                PhotoLenght = 1,
                Price = 1
            });

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Name = "",
                Description = "",
                Photo = new byte[32],
                IsActive = true
            });

            await repository.SaveChangesAsync();

            var products = await marketplaceServices.GetAllAsync(marketplaceId, communityId);

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task TestGetMine()
        {
            var repository = new Repository(context);
            marketplaceServices = new MarketplaceServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid communityId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new ApplicationUser()
            {
                Id = creatorId,
                IsActive = true
            });

            await repository.AddAsync(new Marketplace()
            {
                Id = marketplaceId,
                IsActive = true,
                Name = "",
                CommunityId = communityId
            });

            await repository.AddAsync(new Product()
            {
                Name = "",
                SellerId = creatorId,
                Description = "",
                IsActive = true,
                MarketplaceId = marketplaceId,
                Photo = new byte[32],
                PhotoLenght = 1,
                Price = 1
            });

            await repository.AddAsync(new Product()
            {
                Name = "",
                SellerId = creatorId,
                Description = "",
                IsActive = true,
                MarketplaceId = marketplaceId,
                Photo = new byte[32],
                PhotoLenght = 1,
                Price = 1
            });

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Name = "",
                Description = "",
                Photo = new byte[32],
                IsActive = true
            });

            await repository.SaveChangesAsync();

            var products = await marketplaceServices.GetMineAsync(creatorId ,marketplaceId);

            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task TestDeleteProduct()
        {
            var repository = new Repository(context);
            marketplaceServices = new MarketplaceServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid productId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Product()
            {
                Id = productId,
                Name = "",
                SellerId = creatorId,
                Description = "",
                IsActive = true,
                MarketplaceId = marketplaceId,
                Photo = new byte[32],
                PhotoLenght = 1,
                Price = 1
            });

            await repository.SaveChangesAsync();

            await marketplaceServices.DeleteProductAsync(productId);

            var product = await repository.GetByIdAsync<Product>(productId);

            Assert.That(product.IsActive, Is.False);
        }

        [Test]
        public async Task TestGetProductById()
        {
            var repository = new Repository(context);
            marketplaceServices = new MarketplaceServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid productId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new ApplicationUser()
            {
                Id = creatorId,
                IsActive = true
            });

            await repository.AddAsync(new Product()
            {
                Id = productId,
                Name = "test",
                SellerId = creatorId,
                Description = "test",
                IsActive = true,
                MarketplaceId = marketplaceId,
                Photo = new byte[32],
                PhotoLenght = 1,
                Price = 1
            });

            await repository.SaveChangesAsync();

            var product = await marketplaceServices.GetProductByIdAsync(productId);

            Assert.That(product.Name, Is.Not.Null);
            Assert.That(product.Name, Is.EqualTo("test"));
        }

        [Test]
        public async Task TestEditProduc()
        {
            var repository = new Repository(context);
            marketplaceServices = new MarketplaceServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid productId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Product()
            {
                Id = productId,
                Name = "BeforeEdit",
                SellerId = creatorId,
                Description = "BeforeEdit",
                IsActive = true,
                MarketplaceId = marketplaceId,
                Photo = new byte[32],
                PhotoLenght = 1,
                Price = 1
            });

            await repository.SaveChangesAsync();

            var model = new ManageProductViewModel()
            {
                Name = "AfterEdit",
                Description = "AfterEdit",
                Price = 2
            };

            await marketplaceServices.EditProducAsync(productId, model, new byte[32]);

            var product = await repository.GetByIdAsync<Product>(productId);

            Assert.That(product.Name, Is.EqualTo("AfterEdit"));
            Assert.That(product.Description, Is.EqualTo("AfterEdit"));
            Assert.That(product.Price, Is.EqualTo(2));
        }

        [Test]
        public async Task TestBuyProduct()
        {
            var repository = new Repository(context);
            marketplaceServices = new MarketplaceServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid productId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Product()
            {
                Id = productId,
                Name = "",
                SellerId = creatorId,
                Description = "",
                IsActive = true,
                MarketplaceId = marketplaceId,
                Photo = new byte[32],
                PhotoLenght = 1,
                Price = 1
            });

            await repository.SaveChangesAsync();

            await marketplaceServices.BuyProductAsync(productId, creatorId);

            var product = await repository.GetByIdAsync<Product>(productId);

            Assert.That(product.BuyerId, Is.EqualTo(creatorId));
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
