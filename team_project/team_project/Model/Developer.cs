using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class Developer
    {
        public int DeveloperId { get; set; }
        public string DeveloperName { get; set; } = null;
        public int DeveloperUserId { get; set; }

        public virtual User DeveloperUser { get; set; } = null;
    }
}
