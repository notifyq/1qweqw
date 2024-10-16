using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class User
    {
        public int UserId { get; set; }
        public string UserLogin { get; set; } = null;
        public string UserPassword { get; set; } = null;
        public string UserName { get; set; }
        public string UserEmail { get; set; } = null;
        public string UserImage { get; set; }
        public int UserRole { get; set; }
        public int UserStatus { get; set; }
        public virtual Role UserRoleNavigation { get; set; } = null;
        public virtual Status UserStatusNavigation { get; set; } = null;

    }
}
