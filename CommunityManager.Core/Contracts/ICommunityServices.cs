using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;

namespace CommunityManager.Core.Contracts
{
    public interface ICommunityServices
    {
        Task<CommunityQueryModel> GetAllAsync(string? searchTerm = null, CommunitySorting sorting = CommunitySorting.Newest, int currentPage = 1, int communityPerPage = 1, string currentUserId = "");

        Task<IEnumerable<CommunityViewModel>> GetMineAsync(string id);

        Task<IEnumerable<CommunityViewModel>> GetMineForAdminAsync(string id);

        Task JoinCommunityAsync(Guid communityId, string userId);

        Task<CommunityDetailsViewModel> GetCommunityByIdAsync(Guid id);

        Task<CommunityDetailsViewModel> GetCommunityByIdForAdminAsync(Guid id);

        Task AddMarketplaceToCommunityAsync(AddMarketplaceViewModel model, Guid id);

        Task AddChatroomToCommunityAsync(AddChatroomViewModel model, Guid id, string creatorId);

        Task CreateCommunityAsync(CreateCommunityViewModel model);

        Task ManageCommunityAsync(Guid id, CreateCommunityViewModel model);

        Task DeleteCommunityAsync(Guid communityId);

        Task RestoreCommunityAsync(Guid communityId);

        Task LeaveCommunityAsync(Guid communityId, string userId);

        Task DeleteMarketplaceAsync(Guid marketplaceId);

        Task RestoreMarketplaceAsync(Guid marketplaceId);

        Task DeleteChatroomAsync(Guid chatroomId);

        Task RestoreChatroomAsync(Guid marketplaceId);

        Task<bool> CheckCommunityCreatorId(Guid communityId, string creatorId);

        Task<bool> CheckCommunityMemberId(Guid communityId, string memberId);
    }
}
