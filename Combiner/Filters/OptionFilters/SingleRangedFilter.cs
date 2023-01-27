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
	public class SingleRangedFilter : OptionFilter
	{
		public SingleRangedFilter()
			: base("Single Ranged Attack") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			return !(creature.RangeDamage2 > 0);
		}

		public override BsonExpression BuildQuery()
		{
			return Query.And(
				Query.GT("RangeDamage1", 0),
				Query.EQ("RangeDamage2", 0));
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();

			bq.Add(LuceneService.HasDoubleValue("RangeDamage1"), Occur.MUST);
			bq.Add(LuceneService.HasNoDoubleValue("RangeDamage2"), Occur.MUST);

			return bq;
		}

		public override string ToString()
		{
			return nameof(SingleRangedFilter);
		}
	}
}
