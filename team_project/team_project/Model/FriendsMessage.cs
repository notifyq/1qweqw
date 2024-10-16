using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class FriendsMessage
    {
        public int MessageId { get; set; }
        public int FriendshipId { get; set; }
        public string MessageContent { get; set; } = null;
        public int UserSender { get; set; }
        public DateTime MessageDate { get; set; }
        public sbyte HasBeenRead { get; set; }

        public virtual Friend Friendship { get; set; } = null;
        public virtual User UserSenderNavigation { get; set; } = null;
    }
}
