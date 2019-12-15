using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TableSearch.DomainObjects;
using Newtonsoft.Json;
using System.IO;

namespace TableSearch.MovieSearch
{
    public class MovieRepository
    {
        public static IEnumerable<Movie> GetMoviesFromFile() => JsonConvert.DeserializeObject<List<Movie>>(File.ReadAllText(Settings.MovieJsonFile));
    }
}
