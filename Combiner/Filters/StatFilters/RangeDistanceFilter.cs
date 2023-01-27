using LiteDB;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query = LiteDB.Query;

namespace Combiner
{
	public class RangeDistanceFilter : StatFilter
	{
		public RangeDistanceFilter()
			: base("Range Distance","", 0, 100) { }

		public override bool Filter(Creature creature)
		{
			bool isBothUnderMax = creature.RangeMax1 < (MaxValue + 1)
				&& creature.RangeMax2 < (MaxValue + 1);

			bool isOneOverMin = creature.RangeMax1 >= MinValue
				|| creature.RangeMax2 >= MinValue;

			return isBothUnderMax && isOneOverMin;
		}

		public override BsonExpression BuildQuery()
		{
			var isBothUnderMaxQuery = Query.And(
				Query.LT("RangeMax1", MaxValue + 1),
				Query.LT("RangeMax2", MaxValue + 1));

			var isOneOverMinQuery = Query.Or(
				Query.GTE("RangeMax1", MinValue),
				Query.GTE("RangeMax2", MinValue));

			return Query.And(isBothUnderMaxQuery, isOneOverMinQuery);
		}
		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();

			var isOneOverMinQuery = new BooleanQuery();

			isOneOverMinQuery.Add(NumericRangeQuery.NewDoubleRange("RangeMax1", MinValue, null, true, true), Occur.SHOULD);
			isOneOverMinQuery.Add(NumericRangeQuery.NewDoubleRange("RangeMax2", MinValue, null, true, true), Occur.SHOULD);

			var isBothUnderMaxQuery = new BooleanQuery();

			isBothUnderMaxQuery.Add(NumericRangeQuery.NewDoubleRange("RangeMax1", null, MaxValue + 1, true, false), Occur.MUST);
			isBothUnderMaxQuery.Add(NumericRangeQuery.NewDoubleRange("RangeMax2", null, MaxValue + 1, true, false), Occur.MUST);

			bq.Add(isOneOverMinQuery, Occur.MUST);
			bq.Add(isBothUnderMaxQuery, Occur.MUST);

			return bq;
		}
		public override string ToString()
		{
			return nameof(RangeDistanceFilter);
		}
	}
}
