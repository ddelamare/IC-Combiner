﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Combiner
{
	public class SizeFilter : StatFilter
	{
		public SizeFilter()
			: base("Size", 0, 10) { }

		public override bool Filter(Creature creature)
		{
			return creature.Size >= MinValue
				&& creature.Size < (MaxValue + 1);
		}

		public override Query BuildQuery()
		{
			return Query.Between("Size", MinValue, MaxValue);
		}

		public override string ToString()
		{
			return nameof(SizeFilter);
		}
	}
}
