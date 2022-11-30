using CommunityManager.Core.Models.User;

namespace CommunityManager.Core.Models.Chatroom
{
    public class ChatroomViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public List<MessageViewModel>? Messages { get; set; } = new List<MessageViewModel>();

        public List<UserViewModel>? Members { get; set; } = new List<UserViewModel>();

        public bool IsActive { get; set; }
    }
}
