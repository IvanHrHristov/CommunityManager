using CommunityManager.Core.Models;
using CommunityManager.Core.Models.User;

namespace CommunityManager.Core.Contracts
{
    /// <summary>
    /// Abstraction of community service methods
    /// </summary>
    public interface IUserServices
    {
        /// <summary>
        /// Edits the details of the current user
        /// </summary>
        /// <param name="model">Edit view model</param>
        Task EditUserAsync(EditViewModel model);
    }
}
