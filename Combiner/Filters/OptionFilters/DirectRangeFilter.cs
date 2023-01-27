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
	public class DirectRangeFilter : OptionFilter
	{
		public DirectRangeFilter()
			: base("Direct Range") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			bool range1HasDirect = creature.RangeDamage1 > 0
					&& creature.RangeSpecial1 == 0
					&& (int)creature.RangeType1 != (int)DamageType.Sonic;
			bool range2HasDirect = creature.RangeDamage2 > 0
				&& creature.RangeSpecial2 == 0
				&& (int)creature.RangeType2 != (int)DamageType.Sonic;

			return range1HasDirect || range2HasDirect;
		}

		public override BsonExpression BuildQuery()
		{
			var range1HasDirectQuery = Query.And(
				Query.GT("RangeDamage1", 0),
				Query.EQ("RangeSpecial1", 0),
				Query.Not("RangeType1", (int)DamageType.Sonic));

			var range2HasDirectQuery = Query.And(
				Query.GT("RangeDamage2", 0),
				Query.EQ("RangeSpecial2", 0),
				Query.Not("RangeType2", (int)DamageType.Sonic));

			return Query.Or(range1HasDirectQuery, range2HasDirectQuery);
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();
			var range1HasDirectQuery = new BooleanQuery();
			var range2HasDirectQuery = new BooleanQuery();

			range1HasDirectQuery.Add(LuceneService.HasDoubleValue("RangeDamage1"), Occur.MUST);
			range1HasDirectQuery.Add(LuceneService.HasNoDoubleValue("RangeSpecial1"), Occur.MUST);
			range1HasDirectQuery.Add(LuceneService.HasIntValue("RangeType1", (int)DamageType.Sonic), Occur.MUST_NOT);
						
			range2HasDirectQuery.Add(LuceneService.HasDoubleValue("RangeDamage2"), Occur.MUST);
			range2HasDirectQuery.Add(LuceneService.HasNoDoubleValue("RangeSpecial2"), Occur.MUST);
			range2HasDirectQuery.Add(LuceneService.HasIntValue("RangeType2", (int)DamageType.Sonic), Occur.MUST_NOT);

			bq.Add(range1HasDirectQuery, Occur.SHOULD);
			bq.Add(range2HasDirectQuery, Occur.SHOULD);

			return bq;
		}

		public override string ToString()
		{
			return nameof(DirectRangeFilter);
		}
	}
}
