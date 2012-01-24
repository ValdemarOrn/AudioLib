using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using AudioLib.Plot;

namespace AudioLib.Test
{
	[TestFixture]
	class TestUtils
	{
		[Test]
		public void TestFiltering()
		{
			var kernel = Utils.SincFilter(0.1f, 256, "blackman");

			//Line l = new Line(Utils.Linspace(-2,2,kernel.Length), kernel);
			//var list = new List<Line>() { l };
			//Plot.ShowPlot(list);

			Transfer tf = new Transfer();
			tf.B = kernel;
			//tf.A = new double[1] { 1.0f };

			double[] wave = new double[100000];
			for (int i = 0; i < wave.Length; i++)
			{
				wave[i] = ((i % 2000) < 1000) ? 1 : 0;
			}
			var output = tf.process(wave);

			Line l2 = new Line(Utils.Linspace(0, 2, wave.Length), wave); l2.LinePen = Pens.Blue;
			Line l3 = new Line(Utils.Linspace(0, 2, output.Length), output); l3.LinePen = Pens.Red;
			var list = new List<Line>() { l2, l3 };
			Plot.Plot.ShowPlot(list);

		}

		[Test]
		public void TestSaw()
		{
			var saw = Utils.Saw(1024, 1.0f);

			Line l = new Line(Utils.Linspace(-1, 1, saw.Length), saw); l.LinePen = Pens.Blue;
			Line l2 = new Line(Utils.Linspace(-1, 1, 2), new double[2] { 1, -1 }); l2.LinePen = Pens.Red;
			var list = new List<Line>() { l, l2 };
			Plot.Plot.ShowPlot(list);
		}

		[Test]
		public void TestSquare()
		{
			var sqr = Utils.Square(1024, 64, 1.0f);

			Line l = new Line(Utils.Linspace(-1, 1, sqr.Length), sqr); l.LinePen = Pens.Blue;
			Line l2 = new Line(new double[4] { -1, 0, 0, 1 }, new double[4] { 1, 1, -1, -1 }); l2.LinePen = Pens.Red;
			var list = new List<Line>() { l, l2 };
			Plot.Plot.ShowPlot(list);
		}

		[Test]
		public void TestTriangle()
		{
			var tri = Utils.Triangle(1024, 1.0f);

			Line l = new Line(Utils.Linspace(-1, 1, tri.Length), tri); l.LinePen = Pens.Blue;
			Line l2 = new Line(new double[4] { -1, -0.5f, 0.5f, 1 }, new double[4] { 0, 1, -1, 0 }); l2.LinePen = Pens.Red;
			var list = new List<Line>() { l, l2 };
			Plot.Plot.ShowPlot(list);
		}

		[Test]
		public void TestBrickwall()
		{
			var saw = Utils.Saw(1024, 1.0f);

			var sine1 = Utils.Sinewave(1024, 0, 1 / 1024.0f * 2.0f * (double)Math.PI, 1.0f);
			var sine2 = Utils.Sinewave(1024, 0, 2 / 1024.0f * 2.0f * (double)Math.PI, 1.0f);
			//var sine1 = Utils.Sinewave(1024, 0, 1 / 1024.0f * 2.0f * (double)Math.PI, 1.0f);
			//var sine1 = Utils.Sinewave(1024, 0, 1 / 1024.0f * 2.0f * (double)Math.PI, 1.0f);

			for (int i = 0; i < saw.Length; i++)
				saw[i] = sine1[i] + sine2[i] + 0.02f;

			var kernel = Utils.SincFilter(1/2024.0f, 256, "Blackman");

			var s = new Stopwatch();
			s.Restart();
			var filtered = Conv.ConvSimpleCircular(saw, kernel);
			s.Stop();

			//MessageBox.Show("Millisec: " + s.ElapsedMilliseconds);

			Line l = new Line(Utils.Linspace(-1, 1, saw.Length), saw); l.LinePen = Pens.Blue;
			Line l2 = new Line(Utils.Linspace(-1, 1, filtered.Length), filtered); l2.LinePen = Pens.Red;
			Line l3 = new Line(Utils.Linspace(-1, 1, saw.Length), sine1); l3.LinePen = Pens.Green;
			Line l4 = new Line(Utils.Linspace(-1, 1, saw.Length), sine2); l4.LinePen = Pens.Gray;
			var list = new List<Line>() { l, l2, l3, l4 };
			Plot.Plot.ShowPlot(list);
		}
	}
}
