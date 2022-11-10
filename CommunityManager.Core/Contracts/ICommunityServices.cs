using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;

namespace CommunityManager.Core.Contracts
{
    public interface ICommunityServices
    {
        Task<IEnumerable<CommunityViewModel>> GetAllAsync();

        Task<IEnumerable<CommunityViewModel>> GetMineAsync(string id);

        Task JoinCommunityAsync(Guid communityId, string userId);

        Task<CommunityDetailsViewModel> GetCommunityByIdAsync(Guid id);

        Task AddMarketplaceToCommunityAsync(AddMarketplaceViewModel model, Guid id);

        Task CreateCommunityAsync(CreateCommunityViewModel model);

        Task ManageCommunityAsync(Guid id, CreateCommunityViewModel model);
    }
}
