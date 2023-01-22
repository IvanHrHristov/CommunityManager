using System.ComponentModel.DataAnnotations;
using static CommunityManager.Infrastructure.Data.Constants.MarketplaceConstants;

namespace CommunityManager.Core.Models.Marketplace
{
    public class AddMarketplaceViewModel
    {
        [Required]
        [MaxLength(NameMaxLenght)]
        [MinLength(NameMinLenght)]
        public string Name { get; set; } = null!;

        public Guid CommunityId { get; set; }
    }
}
