using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class SupportRequest
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public int RequestTypeId { get; set; }
        public int RequestStatusId { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestTitle { get; set; } = null;

        public virtual Status RequestStatus { get; set; } = null;
        public virtual RequestType RequestType { get; set; } = null;
        public virtual User User { get; set; } = null;
    }
}
