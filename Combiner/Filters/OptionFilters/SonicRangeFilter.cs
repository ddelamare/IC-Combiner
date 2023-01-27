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
	public class SonicRangeFilter : OptionFilter
	{
		public SonicRangeFilter()
			: base("Sonic Range") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			bool range1HasSonic = creature.RangeSpecial1 == 0
					&& (int)creature.RangeType1 == (int)DamageType.Sonic;
			bool range2HasSonic = creature.RangeSpecial2 == 0
				&& (int)creature.RangeType2 == (int)DamageType.Sonic;

			return range1HasSonic || range2HasSonic;
		}

		public override BsonExpression BuildQuery()
		{
			var range1HasSonicQuery = Query.And(
				Query.EQ("RangeSpecial1", 0),
				Query.EQ("RangeType1", (int)DamageType.Sonic));

			var range2HasSonicQuery = Query.And(
				Query.EQ("RangeSpecial2", 0),
				Query.EQ("RangeType2", (int)DamageType.Sonic));

			return Query.Or(range1HasSonicQuery, range2HasSonicQuery);
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();
			var range1Query = new BooleanQuery();
			var range2Query = new BooleanQuery();

			range1Query.Add(LuceneService.HasNoDoubleValue("RangeSpecial1"), Occur.MUST);
			range1Query.Add(LuceneService.HasIntValue("RangeType1", (int)DamageType.Sonic), Occur.MUST);

			range2Query.Add(LuceneService.HasNoDoubleValue("RangeSpecial2"), Occur.MUST);
			range2Query.Add(LuceneService.HasIntValue("RangeType2", (int)DamageType.Sonic), Occur.MUST);

			bq.Add(range1Query, Occur.SHOULD);
			bq.Add(range2Query, Occur.SHOULD);

			return bq;
		}

		public override string ToString()
		{
			return nameof(SonicRangeFilter);
		}
	}
}
