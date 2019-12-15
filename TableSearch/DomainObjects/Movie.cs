﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TableSearch.DomainObjects
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Rating { get; set; }
        public List<Actor> Actors { get; set; } = new List<Actor>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
