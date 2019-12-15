using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TableSearch.MovieSearch
{
    public static class Settings
    {
        public static string IndexLocation { get; set; } = @"C:\shibuTemp\LuceneIndex";
        public static string MovieJsonFile { get; set; } = @"Data\movies.json";
    }
}
