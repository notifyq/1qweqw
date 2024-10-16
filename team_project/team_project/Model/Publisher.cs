using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class Publisher
    {
        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = null;
        public int PublisherUserId { get; set; }

        public virtual User PublisherUser { get; set; } = null;
    }
}
