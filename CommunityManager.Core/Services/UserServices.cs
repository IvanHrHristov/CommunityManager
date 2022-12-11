using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Identity;

namespace CommunityManager.Core.Services
{
    /// <summary>
    /// Implementation of user service methods
    /// </summary>
    public class UserServices : IUserServices
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;
        /// <summary>
        /// Providing access to the UserManager 
        /// </summary>
        private readonly UserManager<ApplicationUser> userManager;

        public UserServices(
            IRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            this.repository = repository;
            this.userManager = userManager;
        }

        /// <summary>
        /// Edits the details of the current user
        /// </summary>
        /// <param name="model">Edit view model</param>
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
