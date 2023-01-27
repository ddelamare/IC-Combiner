using LiteDB;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Combiner
{
	public class RankFilter : StatFilter
	{
		public RankFilter()
			: base("Level", "Rank", 1, 5) { }

		public override bool Filter(Creature creature)
		{
			return creature.Rank >= MinValue
				&& creature.Rank <= MaxValue;
		}

		public override BsonExpression BuildQuery()
		{
			return LiteDB.Query.And(
				LiteDB.Query.GTE("Rank", MinValue),
				LiteDB.Query.LTE("Rank", MaxValue));
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			return NumericRangeQuery.NewIntRange("Rank", (int)MinValue, (int)MaxValue, true, true);
		}

		public override string ToString()
		{
			return nameof(RankFilter);
		}
	}
}
