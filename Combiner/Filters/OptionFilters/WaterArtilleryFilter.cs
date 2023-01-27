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
	public class WaterArtilleryFilter : OptionFilter
	{
		public WaterArtilleryFilter()
			: base("Water Artillery") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			return creature.RangeSpecial1 == 2 || creature.RangeSpecial2 == 2;
		}

		public override BsonExpression BuildQuery()
		{
			return Query.Or(
				Query.EQ("RangeSpecial1", 2),
				Query.EQ("RangeSpecial2", 2));
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();

			bq.Add(LuceneService.HasIntValue("RangeSpecial1", 2), Occur.SHOULD);
			bq.Add(LuceneService.HasIntValue("RangeSpecial2", 2), Occur.SHOULD);

			return bq;
		}

		public override string ToString()
		{
			return nameof(WaterArtilleryFilter);
		}
	}
}
