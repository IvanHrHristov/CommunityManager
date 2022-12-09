namespace CommunityManager.Core.Models.ShoppingCart
{
    public class ShoppingCartViewModel
    {
        public decimal TotalPrice { get; set; }

        public IEnumerable<ShoppingCartItemViewModel> Items { get; set; } = Enumerable.Empty<ShoppingCartItemViewModel>();
    }
}
