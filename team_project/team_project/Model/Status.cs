using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class Status
    {

        public int StatusId { get; set; }
        public string StatusName { get; set; } = null;
        public int StatusType { get; set; }

        public virtual StatusType StatusTypeNavigation { get; set; } = null;
    }
}
