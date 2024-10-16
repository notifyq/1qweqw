using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null;
        public string ProductDescription { get; set; } = null;
        public decimal ProductPrice { get; set; }
        public int ProductPublisherId { get; set; }
        public int ProductDeveloperId { get; set; }
        public int ProductStatusId { get; set; }
        public bool IsSelected { get; set; } = false;
        public virtual Publisher ProductPublisher { get; set; } = null;
        public virtual Status ProductStatus { get; set; } = null;
        public virtual ICollection<ProductGenre> ProductGenres { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductUpdate> ProductUpdates { get; set; }
        public virtual ICollection<PurchaseList> PurchaseLists { get; set; }

    }
}
