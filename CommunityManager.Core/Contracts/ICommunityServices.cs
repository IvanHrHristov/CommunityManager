using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Models.User;

namespace CommunityManager.Core.Contracts
{
    /// <summary>
    /// Abstraction of community service methods
    /// </summary>
    public interface ICommunityServices
    {
        /// <summary>
        /// All communities matching a criteria
        /// </summary>
        /// <param name="searchTerm">String to be contained in the community name or description </param>
        /// <param name="sorting">Enum that determines how the communities are ordered</param>
        /// <param name="currentPage">Number of the current page</param>
        /// <param name="communityPerPage">Numbers of communities per page</param>
        /// <param name="currentUserId">ID of the current user</param>
        /// <returns>Community query view model</returns>
        Task<CommunityQueryModel> GetAllAsync(string? searchTerm = null, CommunitySorting sorting = CommunitySorting.Newest, int currentPage = 1, int communityPerPage = 1, string currentUserId = "");

        /// <summary>
        /// All communities the current user is a member in
        /// </summary>
        /// <param name="id">ID of the current user</param>
        /// <returns>List of community view models</returns>
        Task<IEnumerable<CommunityViewModel>> GetMineAsync(string id);

        /// <summary>
        /// All communities the current user owns
        /// </summary>
        /// <param name="id">ID of the current user</param>
        /// <returns>List of community view models</returns>
        Task<IEnumerable<CommunityViewModel>> GetMineForAdminAsync(string id);

        /// <summary>
        /// Adds a user to a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        /// <param name="userId">ID of the current user</param>
        Task JoinCommunityAsync(Guid communityId, string userId);

        /// <summary>
        /// Gets a community with a specific ID
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Community view model</returns>
        Task<CommunityDetailsViewModel> GetCommunityByIdAsync(Guid id);

        /// <summary>
        /// Gets a community with a specific ID for admin area
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Community view model</returns>
        Task<CommunityDetailsViewModel> GetCommunityByIdForAdminAsync(Guid id);

        /// <summary>
        /// Adds a marketplace to a community
        /// </summary>
        /// <param name="model">Marketplace view model</param>
        /// <param name="id">ID of the community</param>
        Task AddMarketplaceToCommunityAsync(AddMarketplaceViewModel model, Guid id);

        /// <summary>
        /// Adds a chatroom to a community
        /// </summary>
        /// <param name="model">Chatroom view model</param>
        /// <param name="id">ID of the community</param>
        /// <param name="creatorId">ID of the current user</param>
        Task AddChatroomToCommunityAsync(AddChatroomViewModel model, Guid id, string creatorId);

        /// <summary>
        /// Creates a community
        /// </summary>
        /// <param name="model">Community view model</param>
        Task CreateCommunityAsync(CreateCommunityViewModel model);

        /// <summary>
        /// Edits details of a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <param name="model">Community view model</param>
        Task ManageCommunityAsync(Guid id, CreateCommunityViewModel model);

        /// <summary>
        /// Sets IsActive to false for a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        Task DeleteCommunityAsync(Guid communityId);

        /// <summary>
        /// Sets IsActive to true for a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        Task RestoreCommunityAsync(Guid communityId);

        /// <summary>
        /// Removes current user from a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        /// <param name="userId">ID of the current user</param>
        Task LeaveCommunityAsync(Guid communityId, string userId);

        /// <summary>
        /// Sets IsActive to false for a marketplace 
        /// </summary>
        /// <param name="marketplaceId">ID of the marketplace</param>
        Task DeleteMarketplaceAsync(Guid marketplaceId);

        /// <summary>
        /// Sets IsActive to true for a marketplace 
        /// </summary>
        /// <param name="marketplaceId">ID of the marketplace</param>
        Task RestoreMarketplaceAsync(Guid marketplaceId);

        /// <summary>
        /// Sets IsActive to false for a chatroom
        /// </summary>
        /// <param name="chatroomId">ID of the chatroom</param>
        Task DeleteChatroomAsync(Guid chatroomId);

        /// <summary>
        /// Sets IsActive to true for a chatroom
        /// </summary>
        /// <param name="marketplaceId">ID of the chatroom</param>
        Task RestoreChatroomAsync(Guid marketplaceId);

        /// <summary>
        /// Checks if the current user is the creator of a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        /// <param name="creatorId">ID of the current user</param>
        Task<bool> CheckCommunityCreatorId(Guid communityId, string creatorId);

        /// <summary>
        /// Checks if the current user is a member of a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        /// <param name="memberId">ID of the current user</param>
        Task<bool> CheckCommunityMemberId(Guid communityId, string memberId);
    }
}
