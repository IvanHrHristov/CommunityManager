using System.ComponentModel.DataAnnotations;
using static CommunityManager.Infrastructure.Data.Constants.ChatroomConstants;

namespace CommunityManager.Core.Models.Chatroom
{
    public class AddChatroomViewModel
    {
        [Required]
        [MaxLength(NameMaxLenght)]
        [MinLength(NameMinLenght)]
        public string Name { get; set; } = null!;

        public Guid CommunityId { get; set; }
    }
}
