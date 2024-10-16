using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class ProductGenre
    {
        public int ProductGenreId { get; set; }
        public int ProductId { get; set; }
        public int GenreId { get; set; }

        public virtual Genre Genre { get; set; } = null;
        public virtual Product Product { get; set; } = null;
    }
}
