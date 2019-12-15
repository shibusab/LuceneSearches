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

namespace DocumentSearch
{
    public class PdfDocumentSearcher
    {
        public void CreateIndex()
        {
            var indexDirectory = FSDirectory.Open( new System.IO.DirectoryInfo( Settings.IndexLocation));
            var stdAnalyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            var startTime = DateTime.Now;
            Console.WriteLine("Indexing Started at " + startTime.ToString());
            try
            {
                using (var indexWriter = new IndexWriter(indexDirectory, stdAnalyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    var files = System.IO.Directory.GetFiles(Settings.DataFileLocation, "*.pdf", System.IO.SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        using (var reader = new iTextSharp.text.pdf.PdfReader(file))
                        {
                            var text = new StringBuilder();
                            var totPages = reader.NumberOfPages;
                            for (int pageNo = 1; pageNo <= totPages; pageNo++)
                            {
                                var document = new Lucene.Net.Documents.Document();
                                text.Append(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, pageNo));
                                document.Add(new Field("file", file, Field.Store.YES, Field.Index.ANALYZED));
                                document.Add(new Field("pageno", pageNo.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                                document.Add(new Field("content", text.ToString(), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));

                                indexWriter.AddDocument(document);
                                indexWriter.Optimize();
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to Index {0}", ex.StackTrace.ToString());
            }
            var endTime = DateTime.Now;
            Console.WriteLine("Indexing Completed at " + endTime.ToString());

        }


        public string Search(string searchText)
        {
            var result = string.Empty;
            var indexDirectory = FSDirectory.Open( new System.IO.DirectoryInfo(Settings.IndexLocation));
            var indexReader = IndexReader.Open(indexDirectory, true);
            using (var searcher = new IndexSearcher(indexReader))
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
                var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "content", analyzer);
                var query = parser.Parse(searchText);

                var collector = TopScoreDocCollector.Create(1000, true);
                searcher.Search(query, collector);

                var hits = collector.TopDocs().ScoreDocs;
                var totalHits = hits.Length;

                for (int i = 0; i < totalHits; i++)
                {
                    Document hitDocument = searcher.Doc(hits[i].Doc); //get actual document
                    //result = result + "path" +  hitDocument.Get("path")  +  "  content " + hitDocument.Get("content");
                    result = result + "File Name" + hitDocument.Get("file") + "  Page No : " + hitDocument.Get("pageno") + "\n\r";
                }
            }
            return result;
        }
    }
}
