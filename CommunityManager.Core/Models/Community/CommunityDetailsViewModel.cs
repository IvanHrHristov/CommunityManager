using CommunityManager.Core.Models.Chatroom;
using CommunityManager.Core.Models.Marketplace;
using CommunityManager.Core.Models.User;

namespace CommunityManager.Core.Models.Community
{
    public class CommunityDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public byte[] Photo { get; set; } = null!;

        public int PhotoLenght { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatorId { get; set; } = null!;

        public bool AgeRestricted { get; set; }

        public List<UserViewModel> Members { get; set; } = new List<UserViewModel>();

        public List<MarketplaceViewModel> Marketplaces { get; set; } = new List<MarketplaceViewModel>();

        public List<ChatroomViewModel> Chatrooms { get; set; } = new List<ChatroomViewModel>();
    }
}
