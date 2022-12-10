using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Identity;

namespace CommunityManager.Core.Services
{
    public class UserServices : IUserServices
    {
        private readonly IRepository repository;
        private readonly UserManager<ApplicationUser> userManager;

        public UserServices(
            IRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
        }

        public async Task EditUserAsync(EditViewModel model)
        {
            var user = await repository.GetByIdAsync<ApplicationUser>(model.Id);

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Age = model.Age;

            await userManager.UpdateAsync(user);
        }
    }
}
