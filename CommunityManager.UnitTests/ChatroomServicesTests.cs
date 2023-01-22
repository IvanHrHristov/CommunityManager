using CommunityManager.Core.Contracts;
using CommunityManager.Core.Services;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace CommunityManager.UnitTests
{
    public class ChatroomServicesTests
    {
        private IChatroomServices chatroomServices;
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
        public async Task TestCheckChatroomMemberId()
        {
            var repository = new Repository(context);
            chatroomServices = new ChatroomServices(repository);

            var chatroomId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            var memberId = "cb23536b-dfeb-47de-8a21-2e48ed26f9cd";

            await repository.AddAsync(new ApplicationUser()
            {
                Id = memberId,
                IsActive = true
            });

            await repository.AddAsync(new Chatroom()
            {
                Id = chatroomId,
                Name = "",
                IsActive = true
            });

            await repository.AddAsync(new ChatroomMember()
            {
                ApplicationUserId = memberId,
                ChatroomId = chatroomId
            });

            await repository.SaveChangesAsync();

            Assert.That(await chatroomServices.CheckChatroomMemberId(chatroomId, memberId), Is.True);
            Assert.That(await chatroomServices.CheckChatroomMemberId(chatroomId, ""), Is.False);
        }

        [Test]
        public async Task TestCreateMessage()
        {
            var repository = new Repository(context);
            chatroomServices = new ChatroomServices(repository);

            var chatroomId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            
            await repository.AddAsync(new Chatroom()
            {
                Id = chatroomId,
                Name = "test",
                IsActive = true
            });

            await repository.SaveChangesAsync();

            var chatroom = await repository.All<Chatroom>().Where(m => m.Name == "test").FirstAsync();

            Assert.That(chatroom, Is.Not.Null);
            Assert.That(chatroom.Name, Is.EqualTo("test"));
        }

        [Test]
        public async Task TestGetChatroomById()
        {
            var repository = new Repository(context);
            chatroomServices = new ChatroomServices(repository);

            var chatroomId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");

            await repository.AddAsync(new Chatroom()
            {
                Id = chatroomId,
                Name = "",
                IsActive = true
            });

            await repository.SaveChangesAsync();

            var chatroom = await chatroomServices.GetChatroomByIdAsync(chatroomId);

            Assert.That(chatroom, Is.Not.Null);
        }

        [Test]
        public async Task TestJoinChatroom()
        {
            var repository = new Repository(context);
            chatroomServices = new ChatroomServices(repository);

            var chatroomId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            var memberId = "cb23536b-dfeb-47de-8a21-2e48ed26f9cd";

            await chatroomServices.JoinChatroomAsync(chatroomId, memberId);

            var chatroomMembers = await repository.All<ChatroomMember>().Where(cm => cm.ApplicationUserId == memberId).FirstAsync();

            Assert.That(chatroomMembers, Is.Not.Null);
        }

        [Test]
        public async Task TestLeaveChatroom()
        {
            var repository = new Repository(context);
            chatroomServices = new ChatroomServices(repository);

            var chatroomId = Guid.Parse("b3945c4f-9ae0-48f8-9e4e-d199a2205bf4");
            var memberId = "cb23536b-dfeb-47de-8a21-2e48ed26f9cd";

            await repository.AddAsync(new ChatroomMember()
            {
                ApplicationUserId = memberId,
                ChatroomId = chatroomId
            });

            await repository.SaveChangesAsync();

            await chatroomServices.LeaveChatroomAsync(chatroomId, memberId);

            var chatroomMembers = await repository.All<ChatroomMember>().Where(cm => cm.ApplicationUserId == memberId && cm.ChatroomId == chatroomId).FirstOrDefaultAsync();

            Assert.That(chatroomMembers, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
