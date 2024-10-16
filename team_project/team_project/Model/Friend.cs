using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class Friend
    {
        public int FriendshipId { get; set; }
        public int UserSender { get; set; }
        public int UserRecipient { get; set; }
        public sbyte FriendshipActive { get; set; }

        public virtual User UserRecipientNavigation { get; set; } = null;
        public virtual User UserSenderNavigation { get; set; } = null;
    }
}
