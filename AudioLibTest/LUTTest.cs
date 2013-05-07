using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AudioLib.Modules;

namespace AudioLib.Test
{
	[TestFixture]
	class LUTTest
	{
		[Test]
		public void TestReadFile()
		{
			var l = new LUT();
			l.ReadFile("1N914_tf.txt");
			var v = l.GetValue(8.0f);
			Assert.AreEqual(0.6785316, v, 0.000001);

			v = l.GetValue(0.0f);
			Assert.AreEqual(0.0, v, 0.0003);
		}
	}
}
