using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static CommunityManager.Infrastructure.Data.Constants.ChatroomConstants;

namespace CommunityManager.Infrastructure.Data.Models
{
    public class Chatroom
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(NameMaxLenght)]
        public string Name { get; set; } = null!;

        public List<Message>? Messages { get; set; } = new List<Message>();

        [Required]
        public Guid CommunityId { get; set; }

        [Required]
        [ForeignKey(nameof(CommunityId))]
        public Community Community { get; set; } = null!;
    }
}
