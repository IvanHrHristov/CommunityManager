namespace CommunityManager.Core.Models.Marketplace
{
    public class ProductsQueryModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public byte[] Photo { get; set; } = null!;

        public int PhotoLenght { get; set; }

        public string Seller { get; set; } = null!;

        public string BuyerId { get; set; } = null!;

        public string Buyer { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
