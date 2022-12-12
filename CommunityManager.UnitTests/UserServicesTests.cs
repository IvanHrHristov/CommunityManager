using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models;
using CommunityManager.Core.Services;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CommunityManager.UnitTests
{
    public class UserServicesTests
    {
        private IUserServices userServices;
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
        public void TestCreateUser()
        {
            var repository = new Repository(context);
            userServices = new UserServices(repository);

            var model = new RegisterViewModel()
            {
                Email = "tests@mail.com",
                UserName = "tests",
                Age = 18
            };

            var user = userServices.CreateUserAsync(model);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.IsActive, Is.True);
            Assert.That(user.UserName, Is.EqualTo("tests"));
        }

        [Test]
        public async Task TestGetUser()
        {
            var repository = new Repository(context);
            userServices = new UserServices(repository);

            string userId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";
            string userName = "test";

            await repository.AddAsync(new ApplicationUser()
            {
                Id = userId,
                UserName = userName,
                IsActive = true,
            });

            await repository.SaveChangesAsync();

            var user = await userServices.GetUserAsync(userName);
            var user2 = await userServices.GetUserAsync("");

            Assert.That(user.UserName, Is.EqualTo(userName));
            Assert.That(user2, Is.Null);
        }

        [Test]
        public async Task TestEditUser()
        {
            var repository = new Repository(context);
            userServices = new UserServices(repository);

            string userId = "b3945c4f-9ae0-48f8-9e4e-d199a2205bf4";

            await repository.AddAsync(new ApplicationUser()
            {
                Id = userId,
                UserName = "BeforeEdit",
                Email = "BeforeEdit",
                Age = 19,
                IsActive = true,
            });

            await repository.SaveChangesAsync();

            var user = await userServices.EditUserAsync(new EditViewModel()
            {
                Id = userId,
                UserName = "AfterEdit",
                Email = "AfterEdit",
                Age = 20
            });

            Assert.That(user.UserName, Is.EqualTo("AfterEdit"));
            Assert.That(user.Email, Is.EqualTo("AfterEdit"));
            Assert.That(user.Age, Is.EqualTo(20));
        }

        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }
    }
}
