using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AudioLib;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using AudioLib.Plot;

namespace AudioLib.Test
{
	[TestFixture]
	public class InterpolateTest
	{
		[Test]
		public void TestCubicInterpolate()
		{
			double[] table = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f};

			var dist = Utils.Linspace(0.0f, 1.0f, 100);
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = (double)Interpolate.CubicWrap(dist[i], table);
			}

			Line l = new Line(Utils.Linspace(0, 1, table.Length).Select(x => (double)x).ToArray(), table.Select(x => (double)x).ToArray());
			l.DrawDot = true; l.DrawLine = false; l.DotBrush = Brushes.Red;

			Line l2 = new Line(Utils.Linspace(0, 1, dist.Length).Select(x => (double)x).ToArray(), dist.Select(x => (double)x).ToArray());
			l.DotBrush = Brushes.Blue;

			var list = new List<Line>() { l, l2 };
			Plot.Plot.ShowPlot(list);
		}

		[Test]
		public void TestCosineInterpolate()
		{
			double[] table = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f };

			var dist = Utils.Linspace(0.0f, 1.0f, 100);
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = (double)Interpolate.Cosine(dist[i], table);
			}

			Line l = new Line(Utils.Linspace(0, 1, table.Length).Select(x => (double)x).ToArray(), table.Select(x => (double)x).ToArray());
			l.DrawDot = true; l.DrawLine = false; l.DotBrush = Brushes.Red;

			Line l2 = new Line(Utils.Linspace(0, 1, dist.Length).Select(x => (double)x).ToArray(), dist.Select(x => (double)x).ToArray());
			l.DotBrush = Brushes.Blue;

			var list = new List<Line>() { l, l2 };
			Plot.Plot.ShowPlot(list);
		}

		[Test]
		public void TestSplineInterpolate()
		{
			double[] table = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f };

			var dist = Utils.Linspace(0.0f, 1.0f, 100);
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = (double)Interpolate.Spline(dist[i], table);
			}

			Line l = new Line(Utils.Linspace(0, 1, table.Length).Select(x => (double)x).ToArray(), table.Select(x => (double)x).ToArray());
			l.DrawDot = true; l.DrawLine = false; l.DotBrush = Brushes.Red;

			Line l2 = new Line(Utils.Linspace(0, 1, dist.Length).Select(x => (double)x).ToArray(), dist.Select(x => (double)x).ToArray());
			l.DotBrush = Brushes.Blue;

			var list = new List<Line>() { l, l2 };
			Plot.Plot.ShowPlot(list);
		}

		[Test]
		public void TestSplineWrapInterpolate()
		{
			double[] table = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f };

			var dist = Utils.Linspace(0.0f, 1.0f, 100);
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = (double)Interpolate.SplineWrap(dist[i], table);
			}

			Line l = new Line(Utils.Linspace(0, 1, table.Length).Select(x => (double)x).ToArray(), table.Select(x => (double)x).ToArray());
			l.DrawDot = true; l.DrawLine = false; l.DotBrush = Brushes.Red;

			Line l2 = new Line(Utils.Linspace(0, 1, dist.Length).Select(x => (double)x).ToArray(), dist.Select(x => (double)x).ToArray());
			l.DotBrush = Brushes.Blue;

			var list = new List<Line>() { l, l2 };
			Plot.Plot.ShowPlot(list);
		}

		[Test]
		public void TestInterpolateSpeed()
		{
			double[] table = { 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 0.0f, 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f };
			var s = new Stopwatch();

			var dist = Utils.Linspace(0.0f, 1.0f, 999999);
			s.Restart();
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = (double)Interpolate.Linear(dist[i], table);
			}
			s.Stop();
			var linear = s.ElapsedMilliseconds;

			dist = Utils.Linspace(0.0f, 1.0f, 999999);
			s.Restart();
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = (double)Interpolate.Spline(dist[i], table);
			}
			s.Stop();
			var spline = s.ElapsedMilliseconds;

			dist = Utils.Linspace(0.0f, 1.0f, 999999);
			s.Restart();
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = (double)Interpolate.CubicWrap(dist[i], table);
			}
			s.Stop();
			var cubic = s.ElapsedMilliseconds;

			dist = Utils.Linspace(0.0f, 1.0f, 999999);
			s.Restart();
			for (int i = 0; i < dist.Length; i++)
			{
				dist[i] = (double)Interpolate.Cosine(dist[i], table);
			}
			s.Stop();
			var cosine = s.ElapsedMilliseconds;

			MessageBox.Show("Linear: " + linear + "\nSpline: " + spline + "\nCubic: " + cubic + "\nCosine: " + cosine);
		}
	}
}
