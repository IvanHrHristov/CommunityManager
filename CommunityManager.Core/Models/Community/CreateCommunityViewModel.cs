using System.ComponentModel.DataAnnotations;
using static CommunityManager.Infrastructure.Data.Constants.CommunityConstants;

namespace CommunityManager.Core.Models.Community
{
    public class CreateCommunityViewModel
    {
        [Required]
        [MaxLength(NameMaxLenght)]
        [MinLength(NameMinLenght)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLenght)]
        [MinLength(DescriptionMinLenght)]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public bool AgeRestricted { get; set; }

        [Required]
        public string CreatorId { get; set; } = null!;
    }
}
