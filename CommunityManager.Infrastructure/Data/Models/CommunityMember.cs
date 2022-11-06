using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class CommunityMember
    {
        [Required]
        public string ApplicationUserId { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        public string CommunityId { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(CommunityId))]
        public ApplicationUser Community { get; set; } = null!;
    }
}
