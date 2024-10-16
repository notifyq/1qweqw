using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class ProductImage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ProductImagePath { get; set; } = null;

        public virtual Product Product { get; set; } = null;
    }
}
