using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class SupportRequestMessage
    {
        public int MessageId { get; set; }
        public string MessageContent { get; set; } = null;
        public int UserId { get; set; }
        public DateTime MessageDate { get; set; }
        public int SupportRequestId { get; set; }

        public virtual SupportRequest SupportRequest { get; set; } = null;
        public virtual User User { get; set; } = null;
    }
}
