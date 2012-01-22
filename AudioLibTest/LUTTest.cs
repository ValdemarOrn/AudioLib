using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

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
			l.Read(8.0f);
		}
	}
}
