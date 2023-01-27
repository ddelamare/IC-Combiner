using Combiner.Lucene;
using LiteDB;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query = LiteDB.Query;

namespace Combiner
{
	public class QuillRangeFilter : OptionFilter
	{
		public QuillRangeFilter()
			: base("Quill Range") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			bool range1HasQuill = creature.RangeSpecial1 == 0
					&& (int)creature.RangeType1 == (int)DamageType.Horns;
			bool range2HasQuill = creature.RangeSpecial2 == 0
				&& (int)creature.RangeType2 == (int)DamageType.Horns;

			return range1HasQuill || range2HasQuill;
		}

		public override BsonExpression BuildQuery()
		{
			var range1HasQuillQuery = Query.And(
				Query.EQ("RangeSpecial1", 0),
				Query.EQ("RangeType1", (int)DamageType.Horns));

			var range2HasQuillQuery = Query.And(
				Query.EQ("RangeSpecial2", 0),
				Query.EQ("RangeType2", (int)DamageType.Horns));

			return Query.Or(range1HasQuillQuery, range2HasQuillQuery);
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();
			var range1Query = new BooleanQuery();
			var range2Query = new BooleanQuery();

			range1Query.Add(LuceneService.HasNoDoubleValue("RangeSpecial1"), Occur.MUST);
			range1Query.Add(LuceneService.HasIntValue("RangeType1", (int)DamageType.Horns), Occur.MUST);

			range2Query.Add(LuceneService.HasNoDoubleValue("RangeSpecial2"), Occur.MUST);
			range2Query.Add(LuceneService.HasIntValue("RangeType2", (int)DamageType.Horns), Occur.MUST);

			bq.Add(range1Query, Occur.SHOULD);
			bq.Add(range2Query, Occur.SHOULD);

			return bq;
		}
		public override string ToString()
		{
			return nameof(QuillRangeFilter);
		}
	}
}
