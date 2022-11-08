﻿namespace CommunityManager.Core.Models.Marketplace
{
    public class DetailsProductViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string Description { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public string Seller { get; set; } = null!;
    }
}