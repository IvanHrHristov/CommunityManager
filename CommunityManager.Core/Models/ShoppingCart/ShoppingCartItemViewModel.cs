namespace CommunityManager.Core.Models.ShoppingCart
{
    public class ShoppingCartItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public byte[] Photo { get; set; } = null!;

        public int PhotoLenght { get; set; }
    }
}
