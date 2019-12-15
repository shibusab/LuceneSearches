using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TableSearch.DomainObjects;

namespace TableSearch.MovieSearch
{
    public class MovieSearcher
    {
        //https://www.tutorialspoint.com/lucene/index.htm
        //https://www.tonytruong.net/better-searches-with-lucene-net/
        //https://www.safaribooksonline.com/library/view/windows-developer-power/0596527543/ch04s04.html
        public Lucene.Net.Store.Directory CreateIndex(IEnumerable<Movie> movies)
        {
            if (movies == null) throw new ArgumentNullException();
            var directory = FSDirectory.Open(new System.IO.DirectoryInfo(Settings.IndexLocation));
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            try
            {
                using (var writer = new IndexWriter(directory, analyzer, new IndexWriter.MaxFieldLength(1000)))
                {
                    foreach (var movie in movies)
                    {
                        var doc = new Document();
                        doc.Add(new Field("MovieId", movie.MovieId.ToString(), Field.Store.NO, Field.Index.ANALYZED));
                        doc.Add(new Field("Title", movie.Title.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        doc.Add(new Field("Description", movie.Description.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        //doc.Add(new Field("Actors", movie.Actors, Field.Store.NO, Field.Index.ANALYZED));
                        //doc.Add(new Field("snippet", MakeSnippet("description"), Field.Store.NO, Field.Index.ANALYZED));
                        doc.Add(new Field("Rating", movie.Rating, Field.Store.YES, Field.Index.ANALYZED));
                        writer.AddDocument(doc);
                    }
                    writer.Optimize();
                    writer.Flush(true, true, true);
                }
            }
            finally
            {
                    
            }
            return directory;
        }
        public List<Movie> Search(Lucene.Net.Store.Directory directory, string query)
        {
            var movies = new List<Movie>(); 
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            using (var reader = IndexReader.Open(directory, true))
            {
                using(var searcher = new IndexSearcher(reader))
                {
                    var parsedQuery = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "Title", analyzer)
                        .Parse(query);
                    Console.WriteLine(parsedQuery.ToString());
                    int hitsPerPage = 20;
                    var collector = TopScoreDocCollector.Create(hitsPerPage, true);
                    searcher.Search(parsedQuery, collector);
                    var matches = collector.TopDocs().ScoreDocs;

                    for ( var i =0; i<matches.Length; ++i)
                    {
                        int docId = matches[i].Doc;
                        var document = searcher.Doc(docId);
                        var movie = new Movie();
                        movie.Title= document.GetField("Title").StringValue;
                        movie.Rating= document.GetField("Rating").StringValue;
                        movie.Description= document.GetField("Description").StringValue;
                        
                        movies.Add(movie);
                    }
                }
            }
                return movies;
        }
    }
}
