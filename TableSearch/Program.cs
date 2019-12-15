using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
//using TableSearch.Search;
using System.IO;
using TableSearch.MovieSearch;
using TableSearch.DomainObjects;

//http://programagic.ca/blog/search-with-lucenenet-part-1-basic-implementation
//http://www.thebestcsharpprogrammerintheworld.com/2017/10/12/how-to-create-and-search-a-lucene-net-index-in-4-simple-steps-using-c-step-1/
//http://sonyblogpost.blogspot.com/

namespace TableSearch
{
    class Program
    {
        /// <summary>
        /// deletes all the existing index files
        /// then rebuilds the index and prompts user to search
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DeleteIndexFiles();
            var mySearcher = new MovieSearch.MovieSearcher();
            var directory =mySearcher.CreateIndex(MovieSearch.MovieRepository.GetMoviesFromFile());

            string userInput = string.Empty;
            Console.WriteLine("type quit to exit");
            do
            {
                Console.Write("\nsearch:\\>");
                userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                    continue;
                else if (userInput.Equals("quit"))
                    break;

                var results = mySearcher.Search(directory, userInput);

                DisplayResults(userInput, results);
            } while (true);
        }


        private static void DeleteIndexFiles()
        {
            foreach (FileInfo f in new DirectoryInfo(Settings.IndexLocation).GetFiles())
                f.Delete();
        }

        private static void DisplayResults(string userInput, IEnumerable<Movie> searchResults)
        {
            if (null == searchResults)
            {
                Console.WriteLine($"{userInput} Returned {searchResults.Count()} results\n------------------------------------------" );
                return;
            }
             
            Console.WriteLine($"displaying {searchResults.Count()}  results\n------------------------------------------");
            foreach (var result in searchResults)
                Console.WriteLine($"{result.Title}, {result.Rating} ({result.Description}) \n");
        }
    }
}