using CommunityManager.Core.Models;
using CommunityManager.Core.Models.User;

namespace CommunityManager.Core.Contracts
{
    public interface IUserServices
    {
        Task EditUserAsync(EditViewModel model);
    }
}
