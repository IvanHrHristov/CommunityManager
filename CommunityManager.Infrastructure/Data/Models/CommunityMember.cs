using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class CommunityMember
    {
        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; } = null!;

        [Required]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Community))]
        public Guid CommunityId { get; set; }

        [Required]
        public Community Community { get; set; } = null!;
    }
}
