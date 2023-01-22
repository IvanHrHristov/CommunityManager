using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class ChatroomMember
    {
        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; } = null!;

        [Required]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Chatroom))]
        public Guid ChatroomId { get; set; }

        [Required]
        public Chatroom Chatroom { get; set; } = null!;
    }
}
