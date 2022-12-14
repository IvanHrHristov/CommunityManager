using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    /// <summary>
    /// Implementation of community service methods
    /// </summary>
    public class CommunityServices : ICommunityServices
    {
        /// <summary>
        /// Repository providing access to the database 
        /// </summary>
        private readonly IRepository repository;

        public CommunityServices(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Checks if the current user is the creator of the community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        /// <param name="creatorId">ID of the current user</param>
        public async Task<bool> CheckCommunityCreatorId(Guid communityId, string creatorId)
        {
            var community = await repository.GetByIdAsync<Community>(communityId);

            return (community.CreatorId == creatorId);
        }

        /// <summary>
        /// Checks if the current user is a member of a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        /// <param name="memberId">ID of the current user</param>
        /// <returns>Boolean</returns>
        public async Task<bool> CheckCommunityMemberId(Guid communityId, string memberId)
        {
            var communityMember = repository.AllReadonly<CommunityMember>()
                .Include(cm => cm.Community)
                .Include(cm => cm.ApplicationUser)
                .Where(cm => cm.CommunityId == communityId &&
                    cm.Community.IsActive == true &&
                    cm.ApplicationUser.IsActive == true);

            return await communityMember.AnyAsync(cm => cm.ApplicationUserId == memberId);
        }

        /// <summary>
        /// Adds a marketplace to a community
        /// </summary>
        /// <param name="model">Marketplace view model</param>
        /// <param name="id">ID of the community</param>
        public async Task AddMarketplaceToCommunityAsync(AddMarketplaceViewModel model, Guid id)
        {
            var entity = new Marketplace()
            {
                Name = model.Name,
                CommunityId = id,
                IsActive = true
            };

            await repository.AddAsync(entity);
            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a chatroom to a community
        /// </summary>
        /// <param name="model">Chatroom view model</param>
        /// <param name="id">ID of the community</param>
        /// <param name="creatorId">ID of the current user</param>
        public async Task AddChatroomToCommunityAsync(AddChatroomViewModel model, Guid id, string creatorId)
        {
            var entity = new Chatroom()
            {
                Name = model.Name,
                CommunityId = id,
                IsActive = true
            };

            var chatroomMembers = new ChatroomMember()
            {
                ApplicationUserId = creatorId,
                Chatroom = entity,
            };

            await repository.AddAsync(entity);
            await repository.AddAsync(chatroomMembers);
            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a community
        /// </summary>
        /// <param name="model">Community view model</param>
        public async Task CreateCommunityAsync(CreateCommunityViewModel model)
        {
            var entity = new Community()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedOn = DateTime.Now,
                AgeRestricted = model.AgeRestricted,
                CreatorId = model.CreatorId,
                Photo = model.Photo,
                PhotoLenght = model.PhotoLenght,
                IsActive = true
            };

            var communitiesMembers = new CommunityMember()
            {
                ApplicationUserId = model.CreatorId,
                Community = entity
            };

            await repository.AddAsync(entity);
            await repository.AddAsync(communitiesMembers);
            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a user to a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        /// <param name="userId">ID of the current user</param>
        public async Task JoinCommunityAsync(Guid communityId, string userId)
        {
            var communitiesMembers = new CommunityMember()
            {
                ApplicationUserId = userId,
                CommunityId = communityId
            };

            await repository.AddAsync(communitiesMembers);
            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// All communities matching a criteria
        /// </summary>
        /// <param name="searchTerm">String to be contained in the community name or description </param>
        /// <param name="sorting">Enum that determines how the communities are ordered</param>
        /// <param name="currentPage">Number of the current page</param>
        /// <param name="communityPerPage">Numbers of communities per page</param>
        /// <param name="currentUserId">ID of the current user</param>
        /// <returns>Community query view model</returns>
        public async Task<CommunityQueryModel> GetAllAsync(string? searchTerm = null, CommunitySorting sorting = CommunitySorting.Newest, int currentPage = 1, int communityPerPage = 1, string currentUserId = "")
        {
            var result = new CommunityQueryModel();
            var communities = repository.AllReadonly<Community>()
                .Include(c => c.CommunitiesMembers)
                .ThenInclude(cm => cm.ApplicationUser)
                .Where(c => c.IsActive == true)
                .AsQueryable();

            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                searchTerm = $"%{searchTerm.ToLower()}%";

                communities = communities.Where(c => EF.Functions.Like(c.Name.ToLower(), searchTerm) ||
                    EF.Functions.Like(c.Description.ToLower(), searchTerm));
            }

            communities = sorting switch
            {
                CommunitySorting.Oldest => communities
                    .OrderBy(c => c.CreatedOn)
                    .ThenBy(c => c.Name),
                CommunitySorting.AgeRestricted => communities
                    .OrderBy(c => c.Name).Where(c => c.AgeRestricted == true),
                _ => communities.OrderByDescending(c => c.CreatedOn)
                    .ThenBy(c => c.Name)
            };

            result.Communities = await communities
                .Skip((currentPage - 1) * communityPerPage)
                .Take(communityPerPage)
                .Select(c => new CommunityViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    AgeRestricted = c.AgeRestricted,
                    CreatorId = c.CreatorId,
                    Photo = c.Photo,
                    PhotoLenght = c.PhotoLenght,
                    Members = c.CommunitiesMembers.Select(cm => new UserViewModel() 
                    { 
                        Id = cm.ApplicationUserId,
                        Name = cm.ApplicationUser.UserName
                    }).ToList(),
                    CurrentUserId = currentUserId,
                })
                .ToListAsync();

            result.TotalCommunities = await communities.CountAsync();

            return result;
        }

        /// <summary>
        /// All communities the current user is a member in
        /// </summary>
        /// <param name="id">ID of the current user</param>
        /// <returns>List of community view models</returns>
        public async Task<IEnumerable<CommunityViewModel>> GetMineAsync(string id)
        {
            var entities = await repository.All<Community>()
                .Include(c => c.CommunitiesMembers)
                .ThenInclude(cm => cm.ApplicationUser)
                .Where(cm => cm.CommunitiesMembers.Any(m => m.ApplicationUserId == id) && cm.IsActive == true)
                .ToListAsync();

            return entities.Select(c => new CommunityViewModel()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                AgeRestricted = c.AgeRestricted,
                CreatorId = c.CreatorId,
                Photo = c.Photo,
                PhotoLenght = c.PhotoLenght,
                Members = c.CommunitiesMembers.Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            });
        }

        /// <summary>
        /// All communities the current user owns
        /// </summary>
        /// <param name="id">ID of the current user</param>
        /// <returns>List of community view models</returns>
        public async Task<IEnumerable<CommunityViewModel>> GetMineForAdminAsync(string id)
        {
            var entities = await repository.All<Community>()
                .Include(c => c.CommunitiesMembers)
                .ThenInclude(cm => cm.ApplicationUser)
                .Where(cm => cm.CommunitiesMembers.Any(m => m.ApplicationUserId == id) && 
                    cm.CreatorId == id)
                .OrderByDescending(c => c.IsActive)
                .ToListAsync();

            return entities.Select(c => new CommunityViewModel()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                AgeRestricted = c.AgeRestricted,
                CreatorId = c.CreatorId,
                IsActive = c.IsActive,
                Photo = c.Photo,
                PhotoLenght = c.PhotoLenght,
                Members = c.CommunitiesMembers.Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            });
        }

        /// <summary>
        /// Gets a community with a specific ID
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Community view model</returns>
        public async Task<CommunityDetailsViewModel> GetCommunityByIdAsync(Guid id)
        {
            var entity = await repository.AllReadonly<Community>()
                .Include(c => c.Marketplaces)
                .Include(c => c.Chatrooms)
                .ThenInclude(c => c.ChatroomsMembers)
                .ThenInclude(cm => cm.ApplicationUser)
                .Include(c => c.CommunitiesMembers)
                .ThenInclude(cm => cm.ApplicationUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if(entity == null)
            {
                return new CommunityDetailsViewModel();
            }

            return new CommunityDetailsViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Photo = entity.Photo,
                PhotoLenght = entity.PhotoLenght,
                CreatedOn = entity.CreatedOn,
                CreatorId= entity.CreatorId,
                AgeRestricted = entity.AgeRestricted,
                Marketplaces = entity.Marketplaces.Where(m => m.IsActive).Select(m => new MarketplaceViewModel()
                {
                    Id = m.Id,
                    Name = m.Name,
                    IsActive = m.IsActive
                }).ToList(),
                Chatrooms = entity.Chatrooms.Where(c => c.IsActive).Select(c => new ChatroomViewModel()
                {
                    Id = c.Id,
                    Name= c.Name,
                    IsActive= c.IsActive,
                    Members = c.ChatroomsMembers.Where(cm => cm.ApplicationUser.IsActive).Select(m => new UserViewModel()
                    {
                        Id = m.ApplicationUserId,
                        Name = m.ApplicationUser.UserName
                    }).ToList()
                }).ToList(),
                Members = entity.CommunitiesMembers.Where(cm => cm.ApplicationUser.IsActive).Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            };
        }

        /// <summary>
        /// Gets a community with a specific ID for admin area
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <returns>Community view model</returns>
        public async Task<CommunityDetailsViewModel> GetCommunityByIdForAdminAsync(Guid id)
        {
            var entity = await repository.AllReadonly<Community>()
                .Include(c => c.Marketplaces)
                .Include(c => c.Chatrooms)
                .Include(c => c.CommunitiesMembers)
                .ThenInclude(cm => cm.ApplicationUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return new CommunityDetailsViewModel();
            }

            return new CommunityDetailsViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedOn = entity.CreatedOn,
                CreatorId = entity.CreatorId,
                AgeRestricted = entity.AgeRestricted,
                Marketplaces = entity.Marketplaces.Select(m => new MarketplaceViewModel()
                {
                    Id = m.Id,
                    Name = m.Name,
                    IsActive = m.IsActive
                }).ToList(),
                Chatrooms = entity.Chatrooms.Select(c => new ChatroomViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive
                }).ToList(),
                Members = entity.CommunitiesMembers.Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            };
        }

        /// <summary>
        /// Sets IsActive to false for a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        public async Task DeleteCommunityAsync(Guid communityId)
        {
            var community = await repository.GetByIdAsync<Community>(communityId);

            var chatrooms = await repository.All<Chatroom>()
                .Where(c => c.CommunityId == communityId)
                .ToListAsync();

            var marketplaces = await repository.All<Marketplace>()
                .Where(m => m.CommunityId == communityId)
                .ToListAsync();


            foreach (var marketplace in marketplaces)
            {
                await DeleteMarketplaceAsync(marketplace.Id);
            }

            foreach (var chatroom in chatrooms)
            {
                await DeleteChatroomAsync(chatroom.Id);
            }
            
            community.IsActive = false;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Sets IsActive to true for a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        public async Task RestoreCommunityAsync(Guid communityId)
        {
            var community = await repository.GetByIdAsync<Community>(communityId);

            var chatrooms = await repository.All<Chatroom>()
                .Where(c => c.CommunityId == communityId)
                .ToListAsync();

            var marketplaces = await repository.All<Marketplace>()
                .Where(m => m.CommunityId == communityId)
                .ToListAsync();

            foreach (var marketplace in marketplaces)
            {
                await RestoreMarketplaceAsync(marketplace.Id);
            }

            foreach (var chatroom in chatrooms)
            {
                await RestoreChatroomAsync(chatroom.Id);
            }

            community.IsActive = true;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Sets IsActive to false for a marketplace 
        /// </summary>
        /// <param name="marketplaceId">ID of the marketplace</param>
        public async Task DeleteMarketplaceAsync(Guid marketplaceId)
        {
            var marketplace = await repository.GetByIdAsync<Marketplace>(marketplaceId);

            marketplace.IsActive = false;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Sets IsActive to true for a marketplace 
        /// </summary>
        /// <param name="marketplaceId">ID of the marketplace</param>
        public async Task RestoreMarketplaceAsync(Guid marketplaceId)
        {
            var marketplace = await repository.GetByIdAsync<Marketplace>(marketplaceId);

            marketplace.IsActive = true;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Sets IsActive to false for a chatroom
        /// </summary>
        /// <param name="chatroomId">ID of the chatroom</param>
        public async Task DeleteChatroomAsync(Guid chatroomId)
        {
            var chatroom = await repository.GetByIdAsync<Chatroom>(chatroomId);

            chatroom.IsActive = false;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Sets IsActive to true for a chatroom
        /// </summary>
        /// <param name="marketplaceId">ID of the chatroom</param>
        public async Task RestoreChatroomAsync(Guid chatroomId)
        {
            var chatroom = await repository.GetByIdAsync<Chatroom>(chatroomId);

            chatroom.IsActive = true;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Edits details of a community
        /// </summary>
        /// <param name="id">ID of the community</param>
        /// <param name="model">Community view model</param>
        public async Task ManageCommunityAsync(Guid id, CreateCommunityViewModel model)
        {
            var community = await repository.GetByIdAsync<Community>(id);

            community.Name = model.Name;
            community.Description = model.Description;
            community.CreatedOn = model.CreatedOn;
            community.CreatorId = model.CreatorId;
            community.AgeRestricted = model.AgeRestricted;
            community.Photo = model.Photo;
            community.PhotoLenght = model.PhotoLenght;

            await repository.SaveChangesAsync();
        }

        /// <summary>
        /// Removes current user from a community
        /// </summary>
        /// <param name="communityId">ID of the community</param>
        /// <param name="userId">ID of the current user</param>
        public async Task LeaveCommunityAsync(Guid communityId, string userId)
        {
            var communityMember = await repository.All<CommunityMember>()
                .FirstAsync(cm => cm.CommunityId == communityId && cm.ApplicationUserId == userId);

            repository.Delete(communityMember);
            await repository.SaveChangesAsync();
        }
    }
}
