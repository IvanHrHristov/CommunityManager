using CommunityManager.Core.Contracts;
using CommunityManager.Core.Models.Community;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data;
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

        public async Task<CommunityDetailsViewModel> GetCommunityByIdAsync(Guid id)
        {
            var entity = await context.Communities
                .Include(c => c.Marketplaces)
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
                Members = entity.CommunitiesMembers.Select(cm => new UserViewModel()
                {
                    Id = cm.ApplicationUser.Id,
                    Name = cm.ApplicationUser.UserName
                }).ToList()
            };
        }
    }
}
