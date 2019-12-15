using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DocumentSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            DeleteIndexFiles();
            var mySearcher = new PdfDocumentSearcher();
            mySearcher.CreateIndex();

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

                var results = mySearcher.Search(userInput);

                DisplayResults(userInput, results);
            } while (true);
        }


        private static void DeleteIndexFiles()
        {
            foreach (FileInfo f in new DirectoryInfo(Settings.IndexLocation).GetFiles())
                f.Delete();
        }

        private static void DisplayResults(string userInput, string searchResults)
        {
            if (null == searchResults)
            {
                Console.WriteLine($"{userInput} Returned {searchResults.Count()} results\n------------------------------------------");
                return;
            }

            Console.WriteLine($"displaying {searchResults.Count()}  results\n------------------------------------------");
            Console.WriteLine(searchResults);
        }
    }
}