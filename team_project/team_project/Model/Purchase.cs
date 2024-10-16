using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class Purchase
    {
        public int PurchasesId { get; set; }
        public int UserId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PurchaseStatus { get; set; }

        public virtual Status PurchaseStatusNavigation { get; set; } = null;
        public virtual User User { get; set; } = null;
        public virtual ICollection<PurchaseList> PurchaseLists { get; set; }

    }
}
