﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CommunityManager.Infrastructure.Data.Constants.CommunityConstants;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class Community
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
        public DateTime CreatedOn { get; set; }

        [Required]
        public bool AgeRestricted { get; set; }

        [Required]
        public byte[] Photo { get; set; } = null!;

        [Required]
        public int PhotoLenght { get; set; }

        [Required]
        public string CreatorId { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(CreatorId))]
        public ApplicationUser Creator { get; set; } = null!;

        public List<CommunityMember> CommunitiesMembers { get; set; } = new List<CommunityMember>();

        public List<Marketplace> Marketplaces { get; set; } = new List<Marketplace>();

        public List<Chatroom> Chatrooms { get; set; } = new List<Chatroom>();

        [Required]
        public bool IsActive { get; set; }
    }
}
