using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;

namespace CommunityManager.Core.Contracts
{
    public interface ICommunityServices
    {
        Task<IEnumerable<CommunityViewModel>> GetAllAsync();

        Task<CommunityDetailsViewModel> GetCommunityByIdAsync(Guid id);
    }
}
