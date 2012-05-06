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

			Assert.AreEqual(0.0, map.GetY(0));
			Assert.AreEqual(1.0 / (count - 1), map.GetY(1));
			Assert.AreEqual(2.0 / (count - 1), map.GetY(2));

			map.SetY(0.20, 0);
			map.SetY(0.80, 0);
			map.SetY(0.95, 0);

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
