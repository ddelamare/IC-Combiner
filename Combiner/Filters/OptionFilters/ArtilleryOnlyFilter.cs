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
	public class ArtilleryOnlyFilter : OptionFilter
	{
		public ArtilleryOnlyFilter()
			: base("Artillery Only") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			return creature.RangeSpecial1 > 0 || creature.RangeSpecial2 > 0;
		}

		public override BsonExpression BuildQuery()
		{
			return Query.Or(
				Query.GT("RangeSpecial1", 0),
				Query.GT("RangeSpecial2", 0));
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();
			bq.Add(LuceneService.HasDoubleValue("RangeSpecial1"), Occur.SHOULD);
			bq.Add(LuceneService.HasDoubleValue("RangeSpecial2"), Occur.SHOULD);
			return bq;
		}

		public override string ToString()
		{
			return nameof(ArtilleryOnlyFilter);
		}
	}
}
