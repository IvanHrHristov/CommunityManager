using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CommunityManager.Infrastructure.Data.Constants.ProductConstants;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLenght)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLenght)]
        public string Description { get; set; } = null!;

        [Required]
        [Precision(18,2)]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public string SellerId { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(SellerId))]
        public ApplicationUser Seller { get; set; } = null!;

        public string? BuyerId { get; set; }

        [ForeignKey(nameof(BuyerId))]
        public ApplicationUser? Buyer { get; set; }
    }
}
