using LiteDB;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query = LiteDB.Query;

namespace Combiner
{
	public class RangeDamageFilter : StatFilter
	{
		public RangeDamageFilter()
			: base("Range Damage","", 0, 100) { }

		public override bool Filter(Creature creature)
		{
			bool isBothUnderMax = creature.RangeDamage1 < (MaxValue + 1)
				&& creature.RangeDamage2 < (MaxValue + 1);

			bool isOneOverMin = creature.RangeDamage1 >= MinValue
				|| creature.RangeDamage2 >= MinValue;

			return isBothUnderMax && isOneOverMin;
		}

		public override BsonExpression BuildQuery()
		{
			var isBothUnderMaxQuery = Query.And(
				Query.LT("RangeDamage1", MaxValue + 1),
				Query.LT("RangeDamage2", MaxValue + 1));

			var isOneOverMinQuery = Query.Or(
				Query.GTE("RangeDamage1", MinValue),
				Query.GTE("RangeDamage2", MinValue));

			return Query.And(isBothUnderMaxQuery, isOneOverMinQuery);
		}

		public override global::Lucene.Net.Search.Query BuildLuceneQuery()
		{
			var bq = new BooleanQuery();

			var isOneOverMinQuery = new BooleanQuery();

			isOneOverMinQuery.Add(NumericRangeQuery.NewDoubleRange("RangeDamage1", MinValue, null, true, true), Occur.SHOULD);
			isOneOverMinQuery.Add(NumericRangeQuery.NewDoubleRange("RangeDamage2", MinValue, null, true, true), Occur.SHOULD);

			var isBothUnderMaxQuery = new BooleanQuery();

			isBothUnderMaxQuery.Add(NumericRangeQuery.NewDoubleRange("RangeDamage1", null, MaxValue + 1, true, false), Occur.MUST);
			isBothUnderMaxQuery.Add(NumericRangeQuery.NewDoubleRange("RangeDamage2", null, MaxValue + 1, true, false), Occur.MUST);

			bq.Add(isOneOverMinQuery  , Occur.MUST);
			bq.Add(isBothUnderMaxQuery, Occur.MUST);

			return bq;
		}

		public override string ToString()
		{
			return nameof(RangeDamageFilter);
		}
	}
}
