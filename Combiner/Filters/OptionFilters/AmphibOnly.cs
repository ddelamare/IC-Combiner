using Combiner.Lucene;
using LiteDB;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query = LiteDB.Query;

namespace Combiner
{
	public class AmphibOnlyFilter : OptionFilter
	{
		public AmphibOnlyFilter()
			: base("Amphib Only") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			return creature.LandSpeed > 0
				&& creature.WaterSpeed > 0
				&& creature.AirSpeed == 0;
		}

		public override BsonExpression BuildQuery()
		{
			return Query.And(
				Query.GT("LandSpeed", 0),
				Query.EQ("AirSpeed", 0),
				Query.GT("WaterSpeed", 0));

		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();
			bq.Add(LuceneService.HasDoubleValue("LandSpeed"), Occur.MUST);
			bq.Add(LuceneService.HasDoubleValue("WaterSpeed"), Occur.MUST);
			bq.Add(LuceneService.HasNoDoubleValue("AirSpeed"), Occur.MUST);
			return bq;
		}

		public override string ToString()
		{
			return nameof(AmphibOnlyFilter);
		}
	}
}