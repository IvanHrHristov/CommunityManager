using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static CommunityManager.Infrastructure.Data.Constants.ProductConstants;

namespace CommunityManager.Core.Models.Marketplace
{
    public class ManageProductViewModel
    {
        [Required]
        [MaxLength(NameMaxLenght)]
        [MinLength(NameMinLenght)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLenght)]
        [MinLength(DescriptionMinLenght)]
        public string Description { get; set; } = null!;

        [Required]
        [Precision(18,2)]
        [Range(typeof(decimal), PriceMinValue, PriceMaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        public string SellerId { get; set; } = null!;

        public Guid MarketplaceId { get; set; }
    }
}
