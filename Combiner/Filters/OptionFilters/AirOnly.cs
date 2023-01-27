using Combiner.Lucene;
using LiteDB;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Combiner
{
	public class AirOnlyFilter : OptionFilter
	{
		public AirOnlyFilter()
			: base("Air Only") { }

		protected override bool OnOptionChecked(Creature creature)
		{
			return creature.LandSpeed == 0
				&& creature.WaterSpeed == 0
				&& creature.AirSpeed > 0;
		}

		public override BsonExpression BuildQuery()
		{
			return LiteDB.Query.And(
				LiteDB.Query.EQ("LandSpeed", 0),
				LiteDB.Query.GT("AirSpeed", 0),
				LiteDB.Query.EQ("WaterSpeed", 0));
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();
			bq.Add(LuceneService.HasNoDoubleValue("LandSpeed"), Occur.MUST);
			bq.Add(LuceneService.HasNoDoubleValue("WaterSpeed"), Occur.MUST);
			bq.Add(LuceneService.HasDoubleValue("AirSpeed"), Occur.MUST);
			return bq;
		}

		public override string ToString()
		{
			return nameof(AirOnlyFilter);
		}
	}
}