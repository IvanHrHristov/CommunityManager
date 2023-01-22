namespace CommunityManager.Core.Models.Marketplace
{
    public class DetailsProductViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        public byte[] Photo { get; set; } = null!;

        public int PhotoLenght { get; set; }

        public string Seller { get; set; } = null!;
    }
}
