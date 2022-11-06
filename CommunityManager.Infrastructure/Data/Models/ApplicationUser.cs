using Microsoft.AspNetCore.Identity;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Product>? ProductsPurchased { get; set; } = new List<Product>();

        public List<Product>? ProductsSold { get; set; } = new List<Product>();

        public List<CommunityMember> CommunitiesMembers { get; set; } = new List<CommunityMember>();

        public DateTime CreatedOn { get; set; }
    }
}
