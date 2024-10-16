using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace team_project.Model
{
    public class ProductKey
    {
        public int ProductKeyId { get; set; }
        public int ProductId { get; set; }
        public string Key { get; set; } = null;
        public int UserId { get; set; }

        public virtual Product Product { get; set; } = null;
        public virtual User User { get; set; } = null;
    }
}
