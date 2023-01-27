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
	public class ChemicalArtilleryFilter : OptionFilter
	{
		public ChemicalArtilleryFilter()
			: base("Chemical Artillery") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			return creature.RangeSpecial1 == 3 || creature.RangeSpecial2 == 3;
		}

		public override BsonExpression BuildQuery()
		{
			return Query.Or(
				Query.EQ("RangeSpecial1", 3),
				Query.EQ("RangeSpecial2", 3));
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();
			bq.Add(LuceneService.HasIntValue("RangeSpecial1", 3), Occur.SHOULD);
			bq.Add(LuceneService.HasIntValue("RangeSpecial2", 3), Occur.SHOULD);
			return bq;
		}

		public override string ToString()
		{
			return nameof(ChemicalArtilleryFilter);
		}
	}
}
