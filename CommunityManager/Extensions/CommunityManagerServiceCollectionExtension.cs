using CommunityManager.Core.Contracts;
using CommunityManager.Core.Services;
using HouseRentingSystem.Infrastructure.Data.Common;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommunityManagerServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IMarketplaceServices, MarketplaceServices>();
            services.AddScoped<ICommunityServices, CommunityServices>();
            services.AddScoped<IChatroomServices, ChatroomServices>();
            services.AddScoped<IShoppingCartServices, ShoppingCartServices>();
            services.AddScoped< IUserServices, UserServices> ();

            return services;
        }
    }
}
