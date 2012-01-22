using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AudioLib;
using AudioLib.Plot;

namespace AudioLib.Test
{
	
	[TestFixture]
	class PlotTest
	{
		[Test]
		public void TestPlot()
		{
			Line l = new Line(new double[4] { 1, 2, 9, 2 });
			var list = new List<Line>() { l };
			Plot.Plot.ShowPlot(list);
		}
	}
}
