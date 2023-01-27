using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuceneVersion = Lucene.Net.Util.Version;
using LuceneDirectory = Lucene.Net.Store.Directory;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using static Lucene.Net.Index.IndexWriter;

namespace Combiner.Lucene
{
    public static class LuceneService
    {
        const LuceneVersion luceneVersion = LuceneVersion.LUCENE_30;
        const int MAX_RESULTS = 100;

        private static List<PropertyInfo> creatureFields = null;

        public static void LoadAll()
        {
            var db = new Database();

            foreach (var mod in db.GetAllMods())
            {
                LoadModIndex(mod);
            }
        }

        public static void LoadModIndex(ModCollection mod)
        {
            var db = new Database();

            var allCreatures = db.GetAllCreatures(mod);

            //Open the Directory using a Lucene Directory class
            string indexName = "index_" + mod.CollectionName;
            string indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
            const LuceneVersion luceneVersion = LuceneVersion.LUCENE_30;
            using (LuceneDirectory indexDir = FSDirectory.Open(indexPath))
            {
                //Create an analyzer to process the text 
                Analyzer standardAnalyzer = new StandardAnalyzer(luceneVersion);
                // create/overwrite index
                IndexWriter writer = new IndexWriter(indexDir, standardAnalyzer, true, MaxFieldLength.UNLIMITED);

                foreach (var creature in allCreatures)
                {
                    LuceneService.WriteToIndex(writer, creature);
                }

                writer.Commit();
            }
        }

        // Reflection is slow, so we cache it
        private static List<PropertyInfo> GetFields()
        {
            if (creatureFields != null)
            {
                return creatureFields;
            }

            var fields = typeof(Creature).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            creatureFields = fields.ToList();
            return creatureFields;
        }

        public static Creature MapToCreature(Document doc)
        {
            var creature = new Creature();

            foreach(var field in GetFields())
            {
                var docField = doc.GetField(field.Name);
                if (docField != null)
                {
                    field.SetValue(creature, CoerceValue(field, docField.StringValue));
                }
            }

            return creature;
        }

        private static object CoerceValue(PropertyInfo field, string stringValue)
        {
            // Handle the dictionaries
            if (field.PropertyType.IsGenericType)
            {
                return JsonConvert.DeserializeObject(stringValue, field.PropertyType);
            }
            return Convert.ChangeType(stringValue, field.PropertyType);
        }

        public static void WriteToIndex(IndexWriter writer, Creature creature)
        {
            Document doc = new Document();
            foreach(var field in GetFields())
            {
                // Handle the dictionaries. TODO: Convert to a list format
                if (field.PropertyType.IsGenericType)
                {
                    var serialized = JsonConvert.SerializeObject(field.GetValue(creature));
                    doc.Add(new Field(field.Name, serialized, Field.Store.YES, Field.Index.ANALYZED));
                }
                else if (field.PropertyType == typeof(bool))
                {
                    doc.Add(new Field(field.Name, ((bool)field.GetValue(creature) ? "true" : "false"), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                }
                else if (field.PropertyType.IsValueType)
                {
                    var numField = new NumericField(field.Name, Field.Store.YES, true);
                    if (field.PropertyType == typeof(int))
                    {
                        numField.SetIntValue((int)field.GetValue(creature));
                    }
                    else
                    {
                        numField.SetDoubleValue((double)field.GetValue(creature));
                    }
                    doc.Add(numField);
                }
                else
                {
                    doc.Add(new Field(field.Name, field.GetValue(creature).ToString(), Field.Store.YES, Field.Index.ANALYZED));
                }
            }
            writer.AddDocument(doc);
        }

        public static List<Creature> Query(Query q, ModCollection activeCollection)
        {
            string indexName = "index_" + activeCollection.CollectionName;
            string indexPath = Path.Combine(Environment.CurrentDirectory, indexName);
            using (LuceneDirectory indexDir = FSDirectory.Open(indexPath))
            {
                IndexSearcher searcher = new IndexSearcher(indexDir);
                TopDocs topDocs = searcher.Search(q, n: MAX_RESULTS);
                var docs = new List<Creature>();
                foreach (var doc in topDocs.ScoreDocs)
                {
                    docs.Add(MapToCreature(searcher.Doc(doc.Doc)));
                }
                return docs;
            }
        }

        public static Query HasDoubleValue(string fieldName)
        {
            return NumericRangeQuery.NewDoubleRange(fieldName, 0, null, false, true);
        }
        
        public static Query HasNoDoubleValue(string fieldName)
        {
            return NumericRangeQuery.NewDoubleRange(fieldName, 0, 0, true, true);
        }
        
        public static Query HasIntValue(string fieldName, int val)
        {
            return NumericRangeQuery.NewDoubleRange(fieldName, val, val, true, true);
        }

        public static Query HasBoolValue(string fieldName, bool compareVal)
        {
            return new TermQuery(new Term(fieldName, compareVal ? "true" : "false"));
        }
    }
}
