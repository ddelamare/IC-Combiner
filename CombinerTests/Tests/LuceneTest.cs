using Microsoft.VisualStudio.TestTools.UnitTesting;
using Combiner.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Diagnostics;
using LuceneDirectory = Lucene.Net.Store.Directory;
using System.IO;
using LuceneVersion = Lucene.Net.Util.Version;
using static Lucene.Net.Index.IndexWriter;
using Combiner.Lucene;

namespace Combiner.Tests.Tests
{
    [TestClass()]
    public class LuceneTests
    {
        [TestMethod()]
        public void LoadAll()
        {
            LuceneService.LoadAll();
        }

        [TestMethod]
        public void TryIndex()
        {
            string indexName = "example_index";
            string indexPath = Path.Combine(Environment.CurrentDirectory, indexName);

            using (LuceneDirectory indexDir = FSDirectory.Open(indexPath))
            {
                IndexSearcher searcher = new IndexSearcher(indexDir);
                Query query = new TermQuery(new Term("Rank", "3"));
                TopDocs topDocs = searcher.Search(query, n: 1000);
                var docs = new List<Document>();
                foreach (var doc in topDocs.ScoreDocs)
                {
                    docs.Add(searcher.Doc(doc.Doc));
                }
            }
        }



        [TestMethod]
        public void TryLuceneService()
        {
            Query query = NumericRangeQuery.NewIntRange("Rank", 3, 3, true, true);

            var vals = LuceneService.Query(query);
        }

        [TestMethod]
        public void TestMultiTermQuery()
        {
            var bq = new BooleanQuery();
            var bq2 = new BooleanQuery();
            Query query = NumericRangeQuery.NewIntRange("Rank", 3, 3, true, true);
            Query query2 = NumericRangeQuery.NewDoubleRange("EffectiveHitpoints", 200, 230, true, true);

            bq.Add(query, Occur.MUST);
            bq.Add(query2, Occur.MUST);

            bq2.Add(query, Occur.MUST);

            var v1 = LuceneService.Query(bq);
            var v2 = LuceneService.Query(query);
            var v3 = LuceneService.Query(bq2);
            var vals = LuceneService.Query(query2);
        }
    }
}