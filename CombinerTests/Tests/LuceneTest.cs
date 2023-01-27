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
        public void TryLuceneService()
        {
            var db = new Database();
            var mod = db.GetAllMods().First();
            Query query = NumericRangeQuery.NewIntRange("Rank", 3, 3, true, true);

            var vals = LuceneService.Query(query, mod);
        }

        [TestMethod]
        public void TestMultiTermQuery()
        {
            var db = new Database();
            var mod = db.GetAllMods().First();
            var bq = new BooleanQuery();
            var bq2 = new BooleanQuery();
            Query query = NumericRangeQuery.NewIntRange("Rank", 3, 3, true, true);
            Query query2 = NumericRangeQuery.NewDoubleRange("EffectiveHitpoints", 200, 230, true, true);

            bq.Add(query, Occur.MUST);
            bq.Add(query2, Occur.MUST);

            bq2.Add(query, Occur.MUST);

            var v1 = LuceneService.Query(bq, mod);
            var v2 = LuceneService.Query(query, mod);
            var v3 = LuceneService.Query(bq2, mod);
            var vals = LuceneService.Query(query2, mod);
        }
    }
}