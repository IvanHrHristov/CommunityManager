using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public UserServices(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="model">Register view model</param>
        /// <returns>ApplicationUser</returns>
        public ApplicationUser CreateUserAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.UserName,
                CreatedOn = DateTime.UtcNow,
                Age = model.Age,
                IsActive = true
            };

            return user;
        }

        /// <summary>
        /// Gets a user 
        /// </summary>
        /// <param name="name">Username of the user</param>
        /// <returns>ApplicationUser</returns>
        public async Task<ApplicationUser> GetUserAsync(string name)
        {
            var user = await repository.All<ApplicationUser>().Where(u => u.UserName == name).FirstOrDefaultAsync();

            return user;
        }

        /// <summary>
        /// Edits the details of the current user
        /// </summary>
        /// <param name="model">Edit view model</param>
        public async Task<ApplicationUser> EditUserAsync(EditViewModel model)
        {
            var user = await repository.GetByIdAsync<ApplicationUser>(model.Id);

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Age = model.Age;

            return user;
        }
    }
}
