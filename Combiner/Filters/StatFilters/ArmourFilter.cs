﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Combiner
{
	public class ArmourFilter : StatFilter
	{
		public ArmourFilter()
			: base("Armour", 0, 1.0) { }

		public override bool Filter(Creature creature)
		{
			return creature.Armour >= MinValue
				&& creature.Armour <= MaxValue;
		}

		public override BsonExpression BuildQuery()
		{
			return Query.And(
				Query.GTE("Armour", MinValue),
				Query.LT("Armour", MaxValue + 1));
		}

		public override string ToString()
		{
			return nameof(ArmourFilter);
		}
	}
}
