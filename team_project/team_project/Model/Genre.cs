using System;
using System.Collections.Generic;

namespace team_project.Model
{
    public partial class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; } = null;
        public bool IsSelected { get; set; } = false;
    }
}
