using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Services;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityManager.UnitTests
{
    public class CommunityServicesTests
    {
        private ICommunityServices communityServices;
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
        public async Task TestCheckCommunityCreatorId()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                Name = "",
                Description = "",
                CreatedOn = DateTime.Now,
                AgeRestricted = false,
                CreatorId = creatorId,
                Photo = new Byte[32],
                PhotoLenght = 1,
                IsActive = true
            });

            await repository.SaveChangesAsync();

            Assert.That(await communityServices.CheckCommunityCreatorId(communityId, creatorId), Is.True);
            Assert.That(await communityServices.CheckCommunityCreatorId(communityId, ""), Is.False);
        }

        [Test]
        public async Task TestCheckCommunityMemberId()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new ApplicationUser()
            {
                Id = creatorId,
                IsActive = true
            });

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Description = "",
                Name = "",
                Photo = new Byte[32],
                IsActive = true
            });

            await repository.AddAsync(new CommunityMember()
            {
                ApplicationUserId = creatorId,
                CommunityId = communityId
            });

            await repository.SaveChangesAsync();

            Assert.That(await communityServices.CheckCommunityMemberId(communityId, creatorId), Is.True);
            Assert.That(await communityServices.CheckCommunityMemberId(communityId, ""), Is.False);
        }

        [Test]
        public async Task TestAddMarketplaceToCommunity()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");

            await communityServices.AddMarketplaceToCommunityAsync(new AddMarketplaceViewModel()
            {
                Name = "test"
            }, communityId);

            var marketplace = await repository.All<Marketplace>().Where(m => m.CommunityId == communityId).FirstAsync();

            Assert.That(marketplace, Is.Not.Null);
        }

        [Test]
        public async Task TestAddChatroomToCommunity()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await communityServices.AddChatroomToCommunityAsync(new AddChatroomViewModel()
            {
                Name = "test"
            }, communityId, creatorId);

            var chatroom = await repository.All<Chatroom>().Where(m => m.CommunityId == communityId).FirstAsync();

            Assert.That(chatroom, Is.Not.Null);
        }

        [Test]
        public async Task TestCreateCommunity()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await communityServices.CreateCommunityAsync(new CreateCommunityViewModel()
            {
                CreatorId = creatorId,
                AgeRestricted = false,
                Description = "",
                Photo = new Byte[32],
                PhotoLenght = 1,
                Name = "test"
            });

            var community = await repository.All<Community>().Where(m => m.CreatorId == creatorId).FirstAsync();

            Assert.That(community, Is.Not.Null);
            Assert.That(community.Name, Is.EqualTo("test"));
        }

        [Test]
        public async Task TestJoinCommunity()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await communityServices.JoinCommunityAsync(communityId, creatorId);

            var communitiesMembers = await repository.All<CommunityMember>().Where(m => m.ApplicationUserId == creatorId).FirstAsync();

            Assert.That(communitiesMembers, Is.Not.Null);
        }

        [Test]
        public async Task TestGetAll()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid communityId2 = Guid.Parse("9523b00b-e4cf-4c29-8ff4-cb2ca968fc3d");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";
            string creatorId2 = "cee1a987-641c-45b9-a2a0-2c785ac72362";

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Description = "",
                Name = "",
                Photo = new Byte[32],
                IsActive = true
            });

            await repository.AddAsync(new Community()
            {
                Id = communityId2,
                CreatorId = creatorId2,
                Description = "",
                Name = "",
                Photo = new Byte[32],
                IsActive = true
            });

            await repository.SaveChangesAsync();

            var allCommunities = await communityServices.GetAllAsync();

            Assert.That(allCommunities.TotalCommunities, Is.EqualTo(2));
        }

        [Test]
        public async Task TestGetMine()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid communityId2 = Guid.Parse("9523b00b-e4cf-4c29-8ff4-cb2ca968fc3d");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Description = "",
                Name = "",
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1,
                IsActive = true
            });

            await repository.AddAsync(new Community()
            {
                Id = communityId2,
                CreatorId = creatorId,
                Description = "",
                Name = "",
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1,
                IsActive = true
            });

            await repository.AddAsync(new ApplicationUser
            {
                Id = creatorId,
                UserName = "Admin"
            });

            await repository.AddAsync(new CommunityMember()
            {
                ApplicationUserId = creatorId,
                CommunityId = communityId,
            });

            await repository.AddAsync(new CommunityMember()
            {
                ApplicationUserId = creatorId,
                CommunityId = communityId2
            });

            await repository.SaveChangesAsync();

            var allCommunities = await communityServices.GetMineAsync(creatorId);

            Assert.That(allCommunities.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task TestGetMineForAdmin()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid communityId2 = Guid.Parse("9523b00b-e4cf-4c29-8ff4-cb2ca968fc3d");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Description = "",
                Name = "",
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1,
                IsActive = true
            });

            await repository.AddAsync(new Community()
            {
                Id = communityId2,
                CreatorId = creatorId,
                Description = "",
                Name = "",
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1,
                IsActive = true
            });

            await repository.AddAsync(new ApplicationUser
            {
                Id = creatorId,
                UserName = "Admin"
            });

            await repository.AddAsync(new CommunityMember()
            {
                ApplicationUserId = creatorId,
                CommunityId = communityId,
            });

            await repository.AddAsync(new CommunityMember()
            {
                ApplicationUserId = creatorId,
                CommunityId = communityId2
            });

            await repository.SaveChangesAsync();

            var allCommunities = await communityServices.GetMineForAdminAsync(creatorId);

            Assert.That(allCommunities.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task TestGetCommunityById()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Description = "",
                Name = "",
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1,
                IsActive = true
            });

            await repository.SaveChangesAsync();

            var community = await communityServices.GetCommunityByIdAsync(communityId);

            Assert.That(community, Is.Not.Null);
        }

        [Test]
        public async Task TestDeleteCommunity()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid chatroomId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Description = "",
                Name = "test",
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1,
                IsActive = true
            });

            await repository.AddAsync(new CommunityMember()
            {
                ApplicationUserId = creatorId,
                CommunityId = communityId,
            });

            await repository.AddAsync(new Chatroom()
            {
                Id = chatroomId,
                Name = "",
                IsActive = true,
                CommunityId = communityId
            });

            await repository.AddAsync(new Marketplace()
            {
                Id = marketplaceId,
                IsActive = true,
                Name = "",
                CommunityId = communityId
            });

            await repository.SaveChangesAsync();

            await communityServices.DeleteCommunityAsync(communityId);

            Assert.That((await repository.GetByIdAsync<Community>(communityId)).IsActive, Is.False);
            Assert.That((await repository.GetByIdAsync<Chatroom>(chatroomId)).IsActive, Is.False);
            Assert.That((await repository.GetByIdAsync<Marketplace>(marketplaceId)).IsActive, Is.False);
        }

        [Test]
        public async Task TestRestoreCommunity()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid chatroomId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Description = "",
                Name = "test",
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1,
                IsActive = false
            });

            await repository.AddAsync(new CommunityMember()
            {
                ApplicationUserId = creatorId,
                CommunityId = communityId,
            });

            await repository.AddAsync(new Chatroom()
            {
                Id = chatroomId,
                Name = "",
                IsActive = false,
                CommunityId = communityId
            });

            await repository.AddAsync(new Marketplace()
            {
                Id = marketplaceId,
                IsActive = false,
                Name = "",
                CommunityId = communityId
            });

            await repository.SaveChangesAsync();

            await communityServices.RestoreCommunityAsync(communityId);

            Assert.That((await repository.GetByIdAsync<Community>(communityId)).IsActive, Is.True);
            Assert.That((await repository.GetByIdAsync<Chatroom>(chatroomId)).IsActive, Is.True);
            Assert.That((await repository.GetByIdAsync<Marketplace>(marketplaceId)).IsActive, Is.True);
        }

        [Test]
        public async Task TestDeleteMarketplace()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            
            await repository.AddAsync(new Marketplace()
            {
                Id = marketplaceId,
                IsActive = true,
                Name = ""
            });

            await repository.SaveChangesAsync();

            await communityServices.DeleteMarketplaceAsync(marketplaceId);

            Assert.That((await repository.GetByIdAsync<Marketplace>(marketplaceId)).IsActive, Is.False);
        }

        [Test]
        public async Task TestRestoreMarketplace()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid marketplaceId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");

            await repository.AddAsync(new Marketplace()
            {
                Id = marketplaceId,
                IsActive = false,
                Name = "",
            });

            await repository.SaveChangesAsync();

            await communityServices.RestoreMarketplaceAsync(marketplaceId);

            Assert.That((await repository.GetByIdAsync<Marketplace>(marketplaceId)).IsActive, Is.True);
        }

        [Test]
        public async Task TestDeleteChatroom()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid chatroomId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");

            await repository.AddAsync(new Chatroom()
            {
                Id = chatroomId,
                IsActive = true,
                Name = ""
            });

            await repository.SaveChangesAsync();

            await communityServices.DeleteChatroomAsync(chatroomId);

            Assert.That((await repository.GetByIdAsync<Chatroom>(chatroomId)).IsActive, Is.False);
        }

        [Test]
        public async Task TestRestoreChatroom()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid chatroomId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");

            await repository.AddAsync(new Chatroom()
            {
                Id = chatroomId,
                IsActive = false,
                Name = ""
            });

            await repository.SaveChangesAsync();

            await communityServices.RestoreChatroomAsync(chatroomId);

            Assert.That((await repository.GetByIdAsync<Chatroom>(chatroomId)).IsActive, Is.True);
        }

        [Test]
        public async Task TestManageCommunity()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new Community()
            {
                Id = communityId,
                CreatorId = creatorId,
                Description = "BeforeEdit",
                Name = "BeforeEdit",
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1,
                IsActive = true
            });

            await repository.SaveChangesAsync();

            await communityServices.ManageCommunityAsync(communityId, new CreateCommunityViewModel()
            {
                Name = "AfterEdit",
                Description = "AfterEdit",
                CreatorId = creatorId,
                Photo = new Byte[32],
                AgeRestricted = false,
                PhotoLenght = 1
            });

            Assert.That((await repository.GetByIdAsync<Community>(communityId)).Name, Is.EqualTo("AfterEdit"));
            Assert.That((await repository.GetByIdAsync<Community>(communityId)).Description, Is.EqualTo("AfterEdit"));
        }

        [Test]
        public async Task TestLeaveCommunity()
        {
            var repository = new Repository(context);
            communityServices = new CommunityServices(repository);

            Guid communityId = Guid.Parse("cb23536b-dfeb-47de-8a21-2e48ed26f9cd");
            string creatorId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new CommunityMember()
            {
                ApplicationUserId = creatorId,
                CommunityId = communityId
            });

            await repository.SaveChangesAsync();

            await communityServices.LeaveCommunityAsync(communityId, creatorId);

            var communityMember = await repository.AllReadonly<CommunityMember>()
                .FirstOrDefaultAsync(cm => cm.CommunityId == communityId && cm.ApplicationUserId == creatorId);

            Assert.That(communityMember, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
