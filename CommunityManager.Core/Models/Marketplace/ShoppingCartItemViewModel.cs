﻿namespace CommunityManager.Core.Models.Marketplace
{
    public class ShoppingCartItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = null!;
    }
}
