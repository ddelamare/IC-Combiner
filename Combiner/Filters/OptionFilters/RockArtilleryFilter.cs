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
	public class RockArtilleryFilter : OptionFilter
	{
		public RockArtilleryFilter()
			: base("Rock Artillery") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			return creature.RangeSpecial1 == 1 || creature.RangeSpecial2 == 1;
		}

		public override BsonExpression BuildQuery()
		{
			return Query.Or(
				Query.EQ("RangeSpecial1", 1),
				Query.EQ("RangeSpecial2", 1));
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();

			bq.Add(LuceneService.HasIntValue("RangeSpecial1", 1), Occur.SHOULD);
			bq.Add(LuceneService.HasIntValue("RangeSpecial1", 1), Occur.SHOULD);

			return bq;
		}

		public override string ToString()
		{
			return nameof(RockArtilleryFilter);
		}
	}
}
