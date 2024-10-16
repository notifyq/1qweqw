using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace team_project.Model
{
    public class ProductAdd
    {
        public string ProductName { get; set; } = null;
        public string ProductDescription { get; set; } = null;
        public decimal ProductPrice { get; set; }
        public List<int> GenreIds { get; set; }

    }
}
