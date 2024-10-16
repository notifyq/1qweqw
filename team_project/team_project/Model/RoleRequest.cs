using System;
using System.Collections.Generic;
using team_project.Model;

namespace WebApplication4.Model
{
    public partial class RoleRequest
    {
        public int RoleRequestId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public string UserMessage { get; set; } = null;
        public string AdminNotes { get; set; }
        public DateTime DateRequest { get; set; }
        public DateTime DateLastChanges { get; set; }

        public virtual Role Role { get; set; } = null;
        public virtual Status Status { get; set; } = null;
        public virtual User User { get; set; } = null;
    }
}
