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
	public class PoisonRangeFilter : OptionFilter
	{
		public PoisonRangeFilter()
			: base("Poison Range") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			bool range1HasPoison = creature.RangeSpecial1 == 0
					&& (int)creature.RangeType1 == (int)DamageType.VenomSpray;
			bool range2HasPoison = creature.RangeSpecial2 == 0
				&& (int)creature.RangeType2 == (int)DamageType.VenomSpray;

			return range1HasPoison || range2HasPoison;
		}

		public override BsonExpression BuildQuery()
		{
			var range1HasPoisonQuery = Query.And(
				Query.EQ("RangeSpecial1", 0),
				Query.EQ("RangeType1", (int)DamageType.VenomSpray));

			var range2HasPoisonQuery = Query.And(
				Query.EQ("RangeSpecial2", 0),
				Query.EQ("RangeType2", (int)DamageType.VenomSpray));

			return Query.Or(range1HasPoisonQuery, range2HasPoisonQuery);
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();
			var range1Query = new BooleanQuery();
			var range2Query = new BooleanQuery();

			range1Query.Add(LuceneService.HasNoDoubleValue("RangeSpecial1"), Occur.MUST);
			range1Query.Add(LuceneService.HasIntValue("RangeType1", (int)DamageType.VenomSpray), Occur.MUST);

			range2Query.Add(LuceneService.HasNoDoubleValue("RangeSpecial2"), Occur.MUST);
			range2Query.Add(LuceneService.HasIntValue("RangeType2", (int)DamageType.VenomSpray), Occur.MUST);

			bq.Add(range1Query, Occur.SHOULD);
			bq.Add(range2Query, Occur.SHOULD);

			return bq;
		}

		public override string ToString()
		{
			return nameof(PoisonRangeFilter);
		}
	}
}
