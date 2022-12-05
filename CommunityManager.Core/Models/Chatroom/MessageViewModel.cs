namespace CommunityManager.Core.Models.Chatroom
{
    public class MessageViewModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string SenderId { get; set; } = null!;

        public string SenderUserName { get; set; } = null!;

        public Guid ChatroomId { get; set; }
    }
}
