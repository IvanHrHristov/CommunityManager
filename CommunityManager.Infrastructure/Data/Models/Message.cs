using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CommunityManager.Infrastructure.Data.Constants.MessagesConstants;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(MessageMaxLenght)]
        public string Content { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string SenderId { get; set; } = null!;

        [ForeignKey(nameof(SenderId))]
        public ApplicationUser Sender { get; set; } = null!;

        public Guid ChatroomId { get; set; }

        [ForeignKey(nameof(ChatroomId))]
        public Chatroom Chatroom { get; set; } = null!;
    }
}
