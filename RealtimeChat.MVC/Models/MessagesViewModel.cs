using RtChat.EntityLayer.Concrete;

namespace RealtimeChat.MVC.Models
{
    public class MessagesViewModel
    {
        public List<Messages> ReceivedMessages { get; set; } = new List<Messages>();
        public HashSet<string> DisplayedUsers { get; set; } = new HashSet<string>();
        public List<User>? Users { get; set; }
        public Dictionary<string, List<Messages>>? GroupedMessages { get; set; }
        public User? CurrentUser { get; set; }
        public string? SelectedUser { get; set; } 
    }
}
