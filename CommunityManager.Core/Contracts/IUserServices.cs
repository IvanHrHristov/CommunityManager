using CommunityManager.Core.Models;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data.Models;

namespace CommunityManager.Core.Contracts
{
    /// <summary>
    /// Abstraction of community service methods
    /// </summary>
    public interface IUserServices
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="model">Register view model</param>
        /// <returns>ApplicationUser</returns>
        ApplicationUser CreateUserAsync(RegisterViewModel model);

        /// <summary>
        /// Gets a user 
        /// </summary>
        /// <param name="name">Username of the user</param>
        /// <returns>ApplicationUser</returns>
        Task<ApplicationUser> GetUserAsync(string name);

        /// <summary>
        /// Edits the details of the current user
        /// </summary>
        /// <param name="model">Edit view model</param>
        Task<ApplicationUser> EditUserAsync(EditViewModel model);
    }
}
