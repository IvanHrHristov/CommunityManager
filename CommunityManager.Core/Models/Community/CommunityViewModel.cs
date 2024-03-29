﻿using CommunityManager.Core.Models.User;
using CommunityManager.Infrastructure.Data.Models;

namespace CommunityManager.Core.Models.Community
{
    public class CommunityViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string CreatorId { get; set; } = null!;

        public bool AgeRestricted { get; set; }

        public byte[] Photo { get; set; } = null!;

        public int PhotoLenght { get; set; }

        public List<UserViewModel> Members { get; set; } = new List<UserViewModel>();

        public int TotalCommunityCount { get; set; }

        public string CurrentUserId { get; set; } = null!;

        public bool? IsActive { get; set; }
    }
}