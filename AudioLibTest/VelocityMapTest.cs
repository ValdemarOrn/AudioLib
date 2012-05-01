using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AudioLib.Test
{
	[TestFixture]
	class VelocityMapTest
	{
		[Test]
		public void TestMap()
		{
			int count = 3;
			var map = new VelocityMap(3);

			Assert.AreEqual(0.0, map.Values[0]);
			Assert.AreEqual(1.0/(count-1), map.Values[1]);
			Assert.AreEqual(2.0 / (count-1), map.Values[2]);

			map.Values[0] = 0.2;
			map.Values[1] = 0.8;
			map.Values[2] = 0.95;

			double output = 0.0;
			output = map.Map(-0.1);
			output = map.Map(0.0);
			output = map.Map(0.2);
			output = map.Map(0.4);
			output = map.Map(0.5);
			output = map.Map(0.7);
			output = map.Map(1.0);
			output = map.Map(1.3);
		}
	}
}
