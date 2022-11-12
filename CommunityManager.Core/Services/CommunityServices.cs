using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data;
using CommunityManager.Infrastructure.Data.Models;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CommunityManager.Core.Services
{
    public class CommunityServices : ICommunityServices
    {
        private readonly IRepository repository;
        private readonly ApplicationDbContext context;

        public CommunityServices(
            IRepository repository,
            ApplicationDbContext context)
        {
            this.context = context;
            this.repository = repository;
        }

        public async Task AddMarketplaceToCommunityAsync(AddMarketplaceViewModel model, Guid id)
        {
            var entity = new Marketplace()
            {
                Name = model.Name,
                CommunityId = id,
            };

            await context.Marketplaces.AddAsync(entity);
            await repository.SaveChangesAsync();
        }

        public async Task AddChatroomToCommunityAsync(AddChatroomViewModel model, Guid id, string creatorId)
        {
            var entity = new Chatroom()
            {
                Name = model.Name,
                CommunityId = id
            };

            var chatroomMembers = new ChatroomMember()
            {
                ApplicationUserId = creatorId,
                Chatroom = entity,
            };

            await context.Chatrooms.AddAsync(entity);
            await context.ChatroomsMembers.AddAsync(chatroomMembers);
            await repository.SaveChangesAsync();
        }

        public async Task CreateCommunityAsync(CreateCommunityViewModel model)
        {
            var entity = new Community()
            {
                Name = model.Name,
                Description = model.Description,
                CreatedOn = DateTime.UtcNow,
                AgeRestricted = model.AgeRestricted,
                CreatorId = model.CreatorId
            };

            var communitiesMembers = new CommunityMember()
            {
                ApplicationUserId = model.CreatorId,
                Community = entity
            };

            await context.Communities.AddAsync(entity);
            await context.CommunityMember.AddAsync(communitiesMembers);
            await repository.SaveChangesAsync();
        }


        public async Task JoinCommunityAsync(Guid communityId, string userId)
        {
            var model = await GetCommunityByIdAsync(communityId);

            var community = new Community()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                CreatedOn = model.CreatedOn,
                AgeRestricted = model.AgeRestricted,
                CreatorId = model.CreatorId
            };

            var communitiesMembers = new CommunityMember()
            {
                ApplicationUserId = userId,
                CommunityId = community.Id
            };

            await context.CommunityMember.AddAsync(communitiesMembers);
            await repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommunityViewModel>> GetAllAsync()
        {
            var entities = await context.Communities
                .Include(c => c.CommunitiesMembers)
                .ThenInclude(cm => cm.ApplicationUser)
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

        public async Task<IEnumerable<CommunityViewModel>> GetMineAsync(string id)
        {
            var entities = await context.Communities
                .Include(c => c.CommunitiesMembers)
                .ThenInclude(cm => cm.ApplicationUser)
                .Where(cm => cm.CommunitiesMembers.Any(m => m.ApplicationUserId == id))
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

        public async Task<CommunityDetailsViewModel> GetCommunityByIdAsync(Guid id)
        {
            var entity = await context.Communities
                .Include(c => c.Marketplaces)
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
                Marketplaces = entity.Marketplaces.Select(m => new MarketplaceViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Products = m?.Products?.Select(p => new ProductsQueryModel
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
                Chatrooms = entity.Chatrooms.Select(c => new ChatroomViewModel
                {
                    Id = c.Id,
                    Name= c.Name,
                    Messages = c?.Messages?.Select(m => new MessageViewModel
                    {
                        Id= m.Id,
                        Content = m.Content,
                        SenderId = m.SenderId,
                        Sender = m.Sender.UserName
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

            var communityMembers = await context.CommunityMember
                .Where(cm => cm.CommunityId == communityId)
                .ToListAsync();

            context.CommunityMember.RemoveRange(communityMembers);
            context.Communities.Remove(community);
            await repository.SaveChangesAsync();
        }

        public async Task DeleteMarketplaceAsync(Guid marketplaceId)
        {
            var marketplace = await repository.GetByIdAsync<Marketplace>(marketplaceId);

            var products = await context.Products
                .Where(p => p.MarketplaceId == marketplaceId)
                .ToListAsync();

            context.Products.RemoveRange(products);
            context.Marketplaces.Remove(marketplace);
            await repository.SaveChangesAsync();
        }

        public async Task DeleteChatroomAsync(Guid chatroomId)
        {
            var chatroom = await repository.GetByIdAsync<Chatroom>(chatroomId);

            var chatroomMembers = await context.ChatroomsMembers
                .Where(chm => chm.ChatroomId == chatroomId)
                .ToListAsync();

            context.ChatroomsMembers.RemoveRange(chatroomMembers);
            context.Chatrooms.Remove(chatroom);
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
            var communityMembers = await context.CommunityMember
                .Where(cm => cm.CommunityId == communityId && cm.ApplicationUserId == userId)
                .ToListAsync();

            context.CommunityMember.RemoveRange(communityMembers);
            await repository.SaveChangesAsync();
        }
    }
}
