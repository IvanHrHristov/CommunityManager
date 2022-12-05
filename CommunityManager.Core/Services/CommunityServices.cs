using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    public class CommunityServices : ICommunityServices
    {
        private readonly IRepository repository;
        private readonly RoleManager<IdentityRole> roleManager;

        public CommunityServices(IRepository repository, RoleManager<IdentityRole> roleManager)
        {
            this.repository = repository;
            this.roleManager = roleManager;
        }

        public async Task<bool> CheckCommunityCreatorId(Guid communityId, string creatorId)
        {
            var community = await repository.GetByIdAsync<Community>(communityId);

            return (community.CreatorId == creatorId);
        }

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

        public async Task CreateCommunityAsync(CreateCommunityViewModel model)
        {
            var entity = new Community()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedOn = DateTime.Now,
                AgeRestricted = model.AgeRestricted,
                CreatorId = model.CreatorId,
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

        public async Task<CommunityQueryModel> GetAllAsync(string? searchTerm = null, string? errorMessage = null, CommunitySorting sorting = CommunitySorting.Newest, int currentPage = 1, int communityPerPage = 1, string currentUserId = "placeholder")
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
                    Members = c.CommunitiesMembers.Select(cm => new UserViewModel() 
                    { 
                        Id = cm.ApplicationUserId,
                        Name = cm.ApplicationUser.UserName
                    }).ToList(),
                    ErrorMessage = errorMessage,
                    CurrentUserId = currentUserId,
                })
                .ToListAsync();

            result.TotalCommunities = await communities.CountAsync();

            return result;
        }

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
                Members = c.CommunitiesMembers.Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            });
        }

        public async Task<IEnumerable<CommunityViewModel>> GetMineForAdminAsync(string id)
        {
            var entities = await repository.All<Community>()
                .Include(c => c.CommunitiesMembers)
                .ThenInclude(cm => cm.ApplicationUser)
                .Where(cm => cm.CommunitiesMembers.Any(m => m.ApplicationUserId == id) && 
                    cm.CreatorId == id)
                .ToListAsync();

            return entities.Select(c => new CommunityViewModel()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                AgeRestricted = c.AgeRestricted,
                CreatorId = c.CreatorId,
                IsActive = c.IsActive,
                Members = c.CommunitiesMembers.Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            });
        }

        public async Task<CommunityDetailsViewModel> GetCommunityByIdAsync(Guid id)
        {
            var entity = await repository.All<Community>()
                .Include(c => c.Marketplaces)
                .ThenInclude(m => m.Products)
                .Include(c => c.Chatrooms)
                .ThenInclude(ch => ch.ChatroomsMembers)
                .ThenInclude(chm => chm.ApplicationUser)
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
                CreatedOn = entity.CreatedOn,
                CreatorId= entity.CreatorId,
                AgeRestricted = entity.AgeRestricted,
                Marketplaces = entity.Marketplaces.Where(m => m.IsActive).Select(m => new MarketplaceViewModel()
                {
                    Id = m.Id,
                    Name = m.Name,
                    IsActive = m.IsActive,
                    Products = m?.Products?.Where(p => p.IsActive).Select(p => new ProductsQueryModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        Seller = p.Seller.UserName,
                        BuyerId = p?.BuyerId,
                        Buyer = p?.Buyer?.UserName
                    }).ToList()
                }).ToList(),
                Chatrooms = entity.Chatrooms.Where(c => c.IsActive).Select(c => new ChatroomViewModel()
                {
                    Id = c.Id,
                    Name= c.Name,
                    IsActive= c.IsActive,
                    Messages = c?.Messages?.Select(m => new MessageViewModel()
                    {
                        Id= m.Id,
                        Content = m.Content,
                        SenderId = m.SenderId,
                        ChatroomId = m.ChatroomId,
                        CreatedOn = m.CreatedOn
                    }).ToList(),
                    Members = c?.ChatroomsMembers.Where(chm => chm.ApplicationUser.IsActive).Select(chm => new UserViewModel()
                    {
                        Id = chm.ApplicationUser.Id,
                        Name = chm.ApplicationUser.UserName
                    }).ToList()
                }).ToList(),
                Members = entity.CommunitiesMembers.Where(cm => cm.ApplicationUser.IsActive).Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            };
        }

        public async Task<CommunityDetailsViewModel> GetCommunityByIdForAdminAsync(Guid id)
        {
            var entity = await repository.All<Community>()
                .Include(c => c.Marketplaces)
                .ThenInclude(m => m.Products)
                .Include(c => c.Chatrooms)
                .ThenInclude(ch => ch.ChatroomsMembers)
                .ThenInclude(chm => chm.ApplicationUser)
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
                    IsActive = m.IsActive,
                    Products = m?.Products?.Select(p => new ProductsQueryModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        ImageUrl = p.ImageUrl,
                        Seller = p.Seller.UserName,
                        BuyerId = p?.BuyerId,
                        Buyer = p?.Buyer?.UserName
                    }).ToList()
                }).ToList(),
                Chatrooms = entity.Chatrooms.Select(c => new ChatroomViewModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsActive = c.IsActive,
                    Messages = c?.Messages?.Select(m => new MessageViewModel()
                    {
                        Id = m.Id,
                        Content = m.Content,
                        SenderId = m.SenderId,
                        ChatroomId = m.ChatroomId,
                        CreatedOn = m.CreatedOn
                    }).ToList(),
                    Members = c?.ChatroomsMembers.Select(chm => new UserViewModel()
                    {
                        Id = chm.ApplicationUser.Id,
                        Name = chm.ApplicationUser.UserName
                    }).ToList()
                }).ToList(),
                Members = entity.CommunitiesMembers.Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            };
        }

        public async Task DeleteCommunityAsync(Guid communityId)
        {
            var community = await repository.GetByIdAsync<Community>(communityId);

            //var communityMembers = await repository.All<CommunityMember>()
            //    .Where(cm => cm.CommunityId == communityId)
            //    .ToListAsync();

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
            
            //repository.DeleteRange(communityMembers);
            //await repository.DeleteAsync<Community>(community.Id);

            community.IsActive = false;

            await repository.SaveChangesAsync();
        }

        public async Task RestoreCommunityAsync(Guid communityId)
        {
            var community = await repository.GetByIdAsync<Community>(communityId);

            //var communityMembers = await repository.All<CommunityMember>()
            //    .Where(cm => cm.CommunityId == communityId)
            //    .ToListAsync();

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

            //repository.DeleteRange(communityMembers);
            //await repository.DeleteAsync<Community>(community.Id);

            community.IsActive = true;

            await repository.SaveChangesAsync();
        }

        public async Task DeleteMarketplaceAsync(Guid marketplaceId)
        {
            var marketplace = await repository.GetByIdAsync<Marketplace>(marketplaceId);

            //var products = await repository.All<Product>()
            //    .Where(p => p.MarketplaceId == marketplaceId)
            //    .ToListAsync();

            //repository.DeleteRange(products);
            //await repository.DeleteAsync<Marketplace>(marketplace.Id);

            //foreach (var product in products)
            //{
            //    product.IsActive = false;
            //}

            marketplace.IsActive = false;

            await repository.SaveChangesAsync();
        }

        public async Task RestoreMarketplaceAsync(Guid marketplaceId)
        {
            var marketplace = await repository.GetByIdAsync<Marketplace>(marketplaceId);

            //var products = await repository.All<Product>()
            //    .Where(p => p.MarketplaceId == marketplaceId)
            //    .ToListAsync();

            //repository.DeleteRange(products);
            //await repository.DeleteAsync<Marketplace>(marketplace.Id);

            //foreach (var product in products)
            //{
            //    product.IsActive = true;
            //}

            marketplace.IsActive = true;

            await repository.SaveChangesAsync();
        }

        public async Task DeleteChatroomAsync(Guid chatroomId)
        {
            var chatroom = await repository.GetByIdAsync<Chatroom>(chatroomId);

            //var chatroomMembers = await repository.All<ChatroomMember>()
            //    .Where(chm => chm.ChatroomId == chatroomId)
            //    .ToListAsync();

            //repository.DeleteRange(chatroomMembers);
            //await repository.DeleteAsync<Chatroom>(chatroom.Id);

            chatroom.IsActive = false;

            await repository.SaveChangesAsync();
        }

        public async Task RestoreChatroomAsync(Guid chatroomId)
        {
            var chatroom = await repository.GetByIdAsync<Chatroom>(chatroomId);

            //var chatroomMembers = await repository.All<ChatroomMember>()
            //    .Where(chm => chm.ChatroomId == chatroomId)
            //    .ToListAsync();

            //repository.DeleteRange(chatroomMembers);
            //await repository.DeleteAsync<Chatroom>(chatroom.Id);

            chatroom.IsActive = true;

            await repository.SaveChangesAsync();
        }

        public async Task ManageCommunityAsync(Guid id, CreateCommunityViewModel model)
        {
            var community = await repository.GetByIdAsync<Community>(id);

            community.Name = model.Name;
            community.Description = model.Description;
            community.CreatedOn = model.CreatedOn;
            community.CreatorId = model.CreatorId;
            community.AgeRestricted = model.AgeRestricted;

            await repository.SaveChangesAsync();
        }

        public async Task LeaveCommunityAsync(Guid communityId, string userId)
        {
            var communityMember = await repository.All<CommunityMember>()
                .FirstAsync(cm => cm.CommunityId == communityId && cm.ApplicationUserId == userId);

            repository.Delete(communityMember);
            await repository.SaveChangesAsync();
        }
    }
}
