using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Drawing;
using AudioLib.Plot;

namespace AudioLib.Test
{
	[TestFixture]
	class SimpleDFTTest
	{
		[Test]
		public void TestDFT()
		{
			/*double[] s = Utils.Square(128, 1.0f, 0.5f).Select(x => (double)x).ToArray();
			//s = new double[4] { 0, 1, 0, 0 };
			var d = SimpleDFT.DFT(s);

			for (int i = 64; i < d[0].Length; i++)
			{
				d[0][i] = 0.0;
				d[1][i] = 0.0;
			}

			var output = SimpleDFT.IDFT(d);

			Line l2 = new Line(Utils.Linspace(0, 2, s.Length).Select(x => (double)x).ToArray(), s); 
			l2.DotBrush = Brushes.Blue;
			l2.LinePen = Pens.Blue; 
			l2.DrawDot = true; 
			l2.DrawLine = true;

			Line l3 = new Line(Utils.Linspace(0, 2, output.Length).Select(x => (double)x).ToArray(), output); 
			l3.DotBrush = Brushes.Red;
			l3.LinePen = Pens.Red;
			l3.DrawDot = true;
			l3.DrawLine = true;

			var list = new List<Line>() { l2, l3 };
			Plot.Plot.ShowPlot(list);*/
		}
	}
}
