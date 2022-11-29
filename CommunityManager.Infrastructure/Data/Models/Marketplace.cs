using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CommunityManager.Infrastructure.Data.Constants.MarketplaceConstants;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class Marketplace
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLenght)]
        public string Name { get; set; } = null!;

        public List<Product>? Products { get; set; } = new List<Product>();

        [Required]
        public Guid CommunityId { get; set; }

        [Required]
        [ForeignKey(nameof(CommunityId))]
        public Community Community { get; set; } = null!;

        [Required]
        public bool IsActive { get; set; }
    }
}
