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
	public class RangeOnlyFilter : OptionFilter
	{
		public RangeOnlyFilter()
			: base("Range Only") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			return creature.RangeDamage1 > 0
					&& creature.RangeSpecial1 == 0
					&& creature.RangeSpecial2 == 0;
		}

		public override BsonExpression BuildQuery()
		{
			return Query.And(
				Query.GT("RangeDamage1", 0),
				Query.EQ("RangeSpecial1", 0),
				Query.EQ("RangeSpecial2", 0));
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();

			bq.Add(LuceneService.HasDoubleValue("RangeDamage1"), Occur.MUST);
			bq.Add(LuceneService.HasNoDoubleValue("RangeSpecial1"), Occur.MUST);
			bq.Add(LuceneService.HasNoDoubleValue("RangeSpecial2"), Occur.MUST);

			return bq;
		}

		public override string ToString()
		{
			return nameof(RangeOnlyFilter);
		}
	}
}
