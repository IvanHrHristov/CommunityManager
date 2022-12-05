namespace CommunityManager.Core.Models.Marketplace
{
    public class ShoppingCartViewModel
    {
        public decimal TotalPrice { get; set; }

        public IEnumerable<ShoppingCartItemViewModel> Items { get; set; } = Enumerable.Empty<ShoppingCartItemViewModel>();
    }
}
