using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class Marketplace
    {
        [Key]
        public Guid Id { get; set; }

        public List<Product>? Products { get; set; } = new List<Product>();

        public Guid CommunityId { get; set; }

        public Community Community { get; set; } = null!;
    }
}
